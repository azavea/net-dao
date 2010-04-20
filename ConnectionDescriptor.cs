// Copyright (c) 2004-2010 Azavea, Inc.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Reflection;
using Azavea.Open.Common;
using Azavea.Open.DAO.Exceptions;
using log4net;

namespace Azavea.Open.DAO
{
    /// <summary>
    /// This class represents the information needed to establish a connection to a data source.
    /// This class is abstract, and may be extended to represent a connection to any .NET
    /// data provider (or anything else that makes sense).
    /// 
    /// This class, and any that extend it, should be thread safe.
    /// </summary>
    public abstract class ConnectionDescriptor
    {
        /// <summary>
        /// The log4net logger which child classes may use to log any appropriate messages.
        /// </summary>
        protected static ILog _log = LogManager.GetLogger(
            new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().DeclaringType.Namespace);

        /// <summary>
        /// This is a factory method, that will load the appropriate type of connection
        /// descriptor using the given config.
        /// 
        /// It first searches for config item(s) called "ConnectionConfigSection" and/or
        /// "ConnectionConfig".  (ConnectionConfig should be an "app name" for a config, not a file name).
        /// If present, it will use those to load from another section in this or another
        /// config file.  This allows more dynamic install-time configuration of DB connections.
        /// You may daisy-chain the configuration if you wish.
        /// 
        /// Once in the connection configuration section, it will first search for the "DescriptorClass"
        /// config item, and use that class if specified.  If not, defaults to an OleDbDescriptor
        /// (which means it should be backwards compatible for all our existing config files).
        /// </summary>
        /// <param name="cfg">Config to load the descriptor info from.</param>
        /// <param name="section">What section of that config has the DB connection info in it.</param>
        /// <returns>A fully populated ConnectionDescriptor.</returns>
        public static ConnectionDescriptor LoadFromConfig(Config cfg, string section)
        {
            return LoadFromConfig(cfg, section, null);
        }

        /// <summary>
        /// This is a factory method, that will load the appropriate type of connection
        /// descriptor using the given config.
        /// 
        /// It first searches for config item(s) called "ConnectionConfigSection" and/or
        /// "ConnectionConfig".  (ConnectionConfig should be an "app name" for a config, not a file name).
        /// If present, it will use those to load from another section in this or another
        /// config file.  This allows more dynamic install-time configuration of DB connections.
        /// You may daisy-chain the configuration if you wish.
        /// 
        /// Once in the connection configuration section, it will first search for the "DescriptorClass"
        /// config item, and use that class if specified.  If not, defaults to an OleDbDescriptor
        /// (which means it should be backwards compatible for all our existing config files).
        /// </summary>
        /// <param name="cfg">Config to load the descriptor info from.</param>
        /// <param name="section">What section of that config has the DB connection info in it.</param>
        /// <param name="decryptionDelegate">Method to call to decrypt information, if the actual
        ///                                  connection descriptor type supports decryption.  May be null.</param>
        /// <returns>A fully populated ConnectionDescriptor.</returns>
        public static ConnectionDescriptor LoadFromConfig(Config cfg, string section,
            ConnectionInfoDecryptionDelegate decryptionDelegate)
        {
            if (!cfg.ComponentExists(section))
            {
                throw new BadDaoConfigurationException("Config section " + section +
                                                       " does not exist in " + cfg.Application);
            }
            ConnectionDescriptor retVal;
            // First see if we're redirected to another config and/or section.
            if (cfg.ParameterExists(section, "ConnectionConfig") ||
                cfg.ParameterExists(section, "ConnectionConfigSection"))
            {
                string otherName = cfg.GetParameter(section, "ConnectionConfig", cfg.Application);
                string otherSection = cfg.GetParameter(section, "ConnectionConfigSection", section);
                if (_log.IsDebugEnabled)
                {
                    _log.Debug("Loading " + section + " connection info from "
                               + otherName + "[" + otherSection + "]");
                }
                // Recurse with different config values.
                retVal = LoadFromConfig(Config.GetConfig(otherName), otherSection);
            }
            else
            {
                // Not overridden, read from this config section.
                // For backwards compatibility, default to using an OleDb descriptor.
                string typeName = cfg.GetParameter(section, "DescriptorClass",
                    "Azavea.Open.DAO.OleDb.OleDbDescriptor,Azavea.Open.DAO.OleDb");
                Type[] paramTypes = new Type[] {typeof (Config), typeof (string),
                    typeof(ConnectionInfoDecryptionDelegate)};
                Type descType = Type.GetType(typeName);
                if (descType == null)
                {
                    throw new BadDaoConfigurationException("DescriptorClass '" + typeName +
                                                           "' was specified, but we were unable to get type info.  Are you missing a DLL?");
                }
                ConstructorInfo constr = descType.GetConstructor(paramTypes);
                if (constr == null)
                {
                    throw new BadDaoConfigurationException("DescriptorClass '" + typeName +
                                                           "' was specified, but we were unable to get constructor info.");
                }
                retVal = (ConnectionDescriptor)constr.Invoke(new object[] { cfg, section, decryptionDelegate });
            }
            return retVal;
        }

        /// <summary>
        /// For convenience, this returns ToCleanString().
        /// </summary>
        /// <returns>A string representation of this connection information.</returns>
        public override string ToString()
        {
            return ToCleanString();
        }

        /// <summary>
        /// Since we often need to represent database connection info as strings,
        /// child classes must implement ToCompleteString() such that this.Equals(that) and
        /// this.ToCompleteString().Equals(that.ToCompleteString()) will behave the same.
        /// </summary>
        /// <returns>A string representation of all of the connection info.</returns>
        public abstract string ToCompleteString();

        /// <summary>
        /// This method is similar to ToString, except it will not contain any
        /// "sensitive" information, I.E. passwords.
        /// 
        /// This method is intended to be used for logging or error handling, where
        /// we do not want to display passwords to (potentially) just anyone, but
        /// we do want to indicate what DB connection we were using.
        /// </summary>
        /// <returns>A string representation of most of the connection info, except
        ///          passwords or similar items that shouldn't be shown.</returns>
        public abstract string ToCleanString();

        /// <summary>
        /// Returns the appropriate data access layer for this connection.
        /// </summary>
        public abstract IDaLayer CreateDataAccessLayer();

        /// <summary>
        /// The default implementation does a comparison based on ToCompleteString.  If this is
        /// inaccurate or inefficient for a given implementation, this method should be
        /// overridden.
        /// </summary>
        /// <param name="obj">Other descriptor to compare with.</param>
        /// <returns>True if the two descriptors describe identical connections to a database.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!GetType().Equals(obj.GetType()))
            {
                return false;
            }
            return StringHelper.SafeEquals(ToCompleteString(), ((ConnectionDescriptor)obj).ToCompleteString());
        }

        /// <summary>
        /// The default implementation uses the hashcode of ToCompleteString.  If this is
        /// inaccurate or inefficient for a given implementation, this method should be
        /// overridden.
        /// </summary>
        /// <returns>A semi-unique hash integer.</returns>
        public override int GetHashCode()
        {
            return ToCompleteString().GetHashCode();
        }

        /// <summary>
        /// This method is provided for convenience.  If decryptionDelegate is not null,
        /// will use it to decrypt whatever value is in the config parameter.
        /// </summary>
        /// <param name="config">Config file to get the parameter from.</param>
        /// <param name="component">Section within the config file.</param>
        /// <param name="paramName">Name of the paraneter within the section.</param>
        /// <param name="decryptionDelegate">Method to call to decrypt the parameter.  May be null if using plain text.</param>
        /// <returns></returns>
        protected static string GetDecryptedConfigParameter(Config config,
            string component, string paramName, ConnectionInfoDecryptionDelegate decryptionDelegate)
        {
            string retVal = config.GetParameter(component, paramName, null);
            if (decryptionDelegate != null)
            {
                retVal = decryptionDelegate.Invoke(retVal);
            }
            return retVal;
        }

    }

    /// <summary>
    /// Connection descriptors may support having the connection info stored in the
    /// configuration file in an encrypted format (particularly passwords).  Depending
    /// on the system this may or may not provide actual security benefits, but often
    /// makes people feel better.
    /// 
    /// This is the delegate that the connection descriptor uses to decrypt any encrypted
    /// information (what the descriptor supports having encrypted depends on the
    /// implementation, typically it is just password fields).
    /// </summary>
    /// <param name="input">Encrypted info from the config file.</param>
    /// <returns>The same info in plain text.</returns>
    public delegate string ConnectionInfoDecryptionDelegate(string input);
}