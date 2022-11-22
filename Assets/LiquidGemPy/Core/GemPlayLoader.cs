using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LiquidGemPy.Core
{
    public static class GemPlayLoader
    {
        
        private static readonly Dictionary<string, object> LoadedAddressables = new();
        

        static GemPlayLoader()
        {
            
            
        }

        public static T Addressable<T>(string address)
        {
            T addressable;

#if UNITY_EDITOR
            LoadedAddressables.Clear();
#endif
            
            if (LoadedAddressables.ContainsKey(address))
            {
                addressable = (T) LoadedAddressables[address];
            }
            else
            {
                try
                {
                    addressable = Addressables.LoadAssetAsync<T>(address).WaitForCompletion();
                    LoadedAddressables.Add(address, addressable);
                }
                catch (System.Exception e)
                {
                    addressable = default;
                    
                    Debug.LogException(e);
                }
            }

            return addressable;
        }
    }
}