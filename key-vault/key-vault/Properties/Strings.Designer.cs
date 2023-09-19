﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace key_vault.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("key_vault.Properties.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO Account
        ///(
        ///	Name,
        ///	TenantId,
        ///	ClientId,
        ///	ClientSecret
        ///)
        ///VALUES
        ///(
        ///	@Name,
        ///	@TenantId,
        ///	@ClientId,
        ///	@ClientSecret
        ///).
        /// </summary>
        internal static string AccountService_Create {
            get {
                return ResourceManager.GetString("AccountService_Create", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE Account SET DeletedAt = CURRENT_TIMESTAMP WHERE AccountId = @AccountId AND DeletedAt IS NULL.
        /// </summary>
        internal static string AccountService_Delete {
            get {
                return ResourceManager.GetString("AccountService_Delete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE SecretKey SET DeletedAt = CURRENT_TIMESTAMP WHERE AccountId = @AccountId AND DeletedAt IS NULL.
        /// </summary>
        internal static string AccountService_DeleteAccountSecrets {
            get {
                return ResourceManager.GetString("AccountService_DeleteAccountSecrets", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM Account WHERE AccountId = @AccountId AND DeletedAt IS NULL.
        /// </summary>
        internal static string AccountService_Get {
            get {
                return ResourceManager.GetString("AccountService_Get", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE Account SET ClientSecret = @ClientSecret WHERE AccountId = @AccountId.
        /// </summary>
        internal static string AccountService_UpdateClientSecret {
            get {
                return ResourceManager.GetString("AccountService_UpdateClientSecret", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Server={0};Port={1};User ID={2};Password={3};{4}.
        /// </summary>
        internal static string ConnectionString {
            get {
                return ResourceManager.GetString("ConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE DATABASE IF NOT EXISTS {0}.
        /// </summary>
        internal static string CreateDatabaseString {
            get {
                return ResourceManager.GetString("CreateDatabaseString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to key_vault.
        /// </summary>
        internal static string DatabaseName {
            get {
                return ResourceManager.GetString("DatabaseName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Properties/Scripts/DatabaseScript.sql.
        /// </summary>
        internal static string DatabaseScriptFile {
            get {
                return ResourceManager.GetString("DatabaseScriptFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT LAST_INSERT_ID();.
        /// </summary>
        internal static string MySql_GetLastInsertedId {
            get {
                return ResourceManager.GetString("MySql_GetLastInsertedId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO SecretKey
        ///(
        ///	AccountId,
        ///	Name,
        ///	Description,
        ///	Version,
        ///	Value
        ///)
        ///VALUES
        ///(
        ///	@AccountId,
        ///	@Name,
        ///	@Description,
        ///	@Version,
        ///	@Value
        ///).
        /// </summary>
        internal static string SecretService_Create {
            get {
                return ResourceManager.GetString("SecretService_Create", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE SecretKey SET DeletedAt = CURRENT_TIMESTAMP WHERE AccountId = @AccountId AND Name = @Name AND DeletedAt IS NULL.
        /// </summary>
        internal static string SecretService_Delete {
            get {
                return ResourceManager.GetString("SecretService_Delete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT	SecretKey.*
        ///FROM	SecretKey
        ///	INNER JOIN
        ///	(
        ///		SELECT	AccountId,
        ///			Name,
        ///			MAX(Version) AS LastVersion
        ///		FROM	SecretKey
        ///		GROUP BY
        ///			AccountId,
        ///			Name
        ///	) LastVersion
        ///		ON	SecretKey.AccountId = LastVersion.AccountId AND
        ///			SecretKey.Name = LastVersion.Name AND
        ///			SecretKey.Version = LastVersion.LastVersion
        ///WHERE	SecretKey.AccountId = @AccountId AND
        ///	SecretKey.Name = @Name AND
        /// 	SecretKey.DeletedAt IS NULL.
        /// </summary>
        internal static string SecretService_Get {
            get {
                return ResourceManager.GetString("SecretService_Get", resourceCulture);
            }
        }
    }
}
