﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace key_vault_test.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("key_vault_test.Properties.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to accounts.
        /// </summary>
        internal static string AccountsEndpoint {
            get {
                return ResourceManager.GetString("AccountsEndpoint", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DeletedAt FROM Account WHERE AccountId = @AccountId.
        /// </summary>
        internal static string AccountTest_DeletedAccount {
            get {
                return ResourceManager.GetString("AccountTest_DeletedAccount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM Account WHERE AccountId = @AccountId.
        /// </summary>
        internal static string BaseTest_CleanUpAccount {
            get {
                return ResourceManager.GetString("BaseTest_CleanUpAccount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM SecretKey WHERE AccountId = @AccountId.
        /// </summary>
        internal static string BaseTest_CleanUpSecretKey {
            get {
                return ResourceManager.GetString("BaseTest_CleanUpSecretKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to testsettings.json.
        /// </summary>
        internal static string BaseTest_Initialize_AppSettings {
            get {
                return ResourceManager.GetString("BaseTest_Initialize_AppSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to do action in key-vault &quot;{0}&quot; from tenant &quot;{1}&quot; in &quot;{2}&quot;..
        /// </summary>
        internal static string KeyVault_UnableToDoAction {
            get {
                return ResourceManager.GetString("KeyVault_UnableToDoAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT DeletedAt FROM SecretKey WHERE Name = @name.
        /// </summary>
        internal static string SecretTest_DeletedSecret {
            get {
                return ResourceManager.GetString("SecretTest_DeletedSecret", resourceCulture);
            }
        }
    }
}
