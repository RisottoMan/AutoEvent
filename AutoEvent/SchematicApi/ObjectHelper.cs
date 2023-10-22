using AdminToys;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration.Distributors;
using Mirror;
using UnityEngine;

namespace MER.Lite
{
    public static class ObjectHelper
    {
        private static PrimitiveObjectToy primitiveBaseObject;

        private static LightSourceToy lightBaseObject;

        private static WorkstationController workstationObject;

        private static Locker standardLockerObject;

        private static Locker largeGunLockerObject;

        private static Locker scpPedestalObject;

        private static Locker smallWallCabinetObject;

        public static PrimitiveObjectToy PrimitiveBaseObject
        {
            get
            {
                if ((object)primitiveBaseObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<PrimitiveObjectToy>(out var component))
                        {
                            primitiveBaseObject = component;
                            break;
                        }
                    }
                }

                return primitiveBaseObject;
            }
        }

        public static LightSourceToy LightBaseObject
        {
            get
            {
                if ((object)lightBaseObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<LightSourceToy>(out var component))
                        {
                            lightBaseObject = component;
                            break;
                        }
                    }
                }

                return lightBaseObject;
            }
        }

        public static WorkstationController WorkstationObject
        {
            get
            {
                if ((object)workstationObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<WorkstationController>(out var component))
                        {
                            workstationObject = component;
                            break;
                        }
                    }
                }

                return workstationObject;
            }
        }

        public static Locker StandartLockerObject
        {
            get
            {
                if ((object)standardLockerObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<Locker>(out var component))
                        {
                            if (component.StructureType == StructureType.StandardLocker)
                            {
                                standardLockerObject = component;
                                break;
                            }
                        }
                    }
                }

                return standardLockerObject;
            }
        }

        public static Locker LargeGunLockerObject
        {
            get
            {
                if ((object)largeGunLockerObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<Locker>(out var component))
                        {
                            if (component.StructureType == StructureType.LargeGunLocker)
                            {
                                largeGunLockerObject = component;
                                break;
                            }
                        }
                    }
                }

                return largeGunLockerObject;
            }
        }

        public static Locker ScpPedestalObject
        {
            get
            {
                if ((object)scpPedestalObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<Locker>(out var component))
                        {
                            if (component.StructureType == StructureType.ScpPedestal)
                            {
                                scpPedestalObject = component;
                                break;
                            }
                        }
                    }
                }

                return scpPedestalObject;
            }
        }

        public static Locker SmallWallCabinetObject
        {
            get
            {
                if ((object)smallWallCabinetObject == null)
                {
                    foreach (GameObject value in NetworkClient.prefabs.Values)
                    {
                        if (value.TryGetComponent<Locker>(out var component))
                        {
                            if (component.StructureType == StructureType.SmallWallCabinet)
                            {
                                smallWallCabinetObject = component;
                                break;
                            }
                        }
                    }
                }

                return smallWallCabinetObject;
            }
        }
    }
}