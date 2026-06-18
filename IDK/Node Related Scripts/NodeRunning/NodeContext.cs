using AC.Node_Related_Scripts.ConnectionStuff;
using AC.Node_Related_Scripts.NodeRunning.Instructions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RootMotion.FinalIK.IKSolverVR;

namespace AC.Node_Related_Scripts.NodeRunning
{
    public class NodeContext
    {
        public struct Furniture
        {
            public object value; // Note: Storing primitives here still causes Mono boxing.
            public NodeRegistery.Register register;
        }

        public struct Room
        {
            public ConnectionType.VirtualPort connectionType;
            public int headFurnitureIndex; // Points to master pool (-1 if empty)
            public int furnitureCount;
        }

        public struct House
        {
            public VirtualNode owner;
            public int roomStartIndex;     // Where this house's rooms begin in the flat array
            public int roomCount;
        }

        public NodeContextManager contextManager;
        public NodeContext[] children;

        private House[] _houses;
        private Room[] _allRooms;
        private List<FurnitureElement> _furniturePool = new List<FurnitureElement>(128);

        // --- Read-Only Runtime Lookups (Allocated ONCE in constructor) ---
        private Dictionary<VirtualNode, int> _houseLookup;
        private Dictionary<ConnectionType.VirtualPort, int> _roomLookup;

        public struct FurnitureElement
        {
            public Furniture furniture;
            public int nextIndex;
        }

        public House[] houses => _houses;
        public Room[] rooms => _allRooms;

        public NodeContext(VirtualNode[] virtualNodes, NodeContextManager contextManager)
        {
            
            this.contextManager = contextManager;

            _houseLookup = new Dictionary<VirtualNode, int>(virtualNodes.Length);

            // Calculate total rooms upfront
            int totalRoomsCount = 0;
            for (int i = 0; i < virtualNodes.Length; i++)
            {
                totalRoomsCount += virtualNodes[i].connectionTypes.Count;
            }

            _roomLookup = new Dictionary<ConnectionType.VirtualPort, int>(totalRoomsCount);
            _houses = new House[virtualNodes.Length];
            _allRooms = new Room[totalRoomsCount];

            int currentRoomIndex = 0;

            for (int i = 0; i < virtualNodes.Length; i++)
            {
                var node = virtualNodes[i];
                contextManager.contextCache.Add(node, this);
                _houses[i] = new House
                {
                    owner = node,
                    roomStartIndex = currentRoomIndex,
                    roomCount = node.connectionTypes.Count
                };

                // Cache the index mapping safely at startup
                _houseLookup.Add(node, i);

                for (int j = 0; j < node.connectionTypes.Count; j++)
                {
                    var connectionType = node.connectionTypes[j];

                    _allRooms[currentRoomIndex] = new Room
                    {
                        connectionType = connectionType.first,
                        headFurnitureIndex = -1,
                        furnitureCount = 0
                    };

                    _roomLookup.Add(connectionType.first, currentRoomIndex);
                    currentRoomIndex++;
                }
            }
            if (!contextManager.AllContexts.Contains(this))
                contextManager.AllContexts.Add(this);
        }

        // --- Restored & Optimized Public API ---

        public bool HasHouse(VirtualNode node)
        {
            return _houseLookup.ContainsKey(node);
        }

        public House GetHouse(VirtualNode node)
        {
            if (!_houseLookup.TryGetValue(node, out int index))
            {
                Debug.Log("No house found for:" + node + "in node context:" + this);
                return default;
            }
            return _houses[index];
        }

        // Updated to return our allocation-free structural collection instead of List<Furniture>
        public FurnitureEnumerable GetFurnitures(ConnectionType.VirtualPort connectionType)
        {
            if (!_roomLookup.TryGetValue(connectionType, out int globalRoomIndex))
            {
                Debug.Log("No furniture found for:" + connectionType + "in node context:" + this);
                return new FurnitureEnumerable(null, -1);
            }

            Room room = _allRooms[globalRoomIndex];
            return new FurnitureEnumerable(_furniturePool, room.headFurnitureIndex);
        }

        public void AddFurnitureToRoom(ConnectionType.VirtualPort connectionType, Furniture item)
        {
            if (!_roomLookup.TryGetValue(connectionType, out int globalRoomIndex)) return;

            Room room = _allRooms[globalRoomIndex];

            int newElementIndex = _furniturePool.Count;
            _furniturePool.Add(new FurnitureElement
            {
                furniture = item,
                nextIndex = room.headFurnitureIndex
            });

            room.headFurnitureIndex = newElementIndex;
            room.furnitureCount++;

            _allRooms[globalRoomIndex] = room;
        }
        public void ClearFurnitureOfRoom(ConnectionType.VirtualPort connectionType)
        {
            if (!_roomLookup.TryGetValue(connectionType, out int globalRoomIndex)) return;

            Room room = _allRooms[globalRoomIndex];

            _furniturePool.Clear();
            room.headFurnitureIndex = 0;
            room.furnitureCount = 0;

            _allRooms[globalRoomIndex] = room;
        }
        public Furniture AllocateObject(object val)
        {
            NodeRegistery.Register register = contextManager.Registery.AllocateMemoryToRegister();
            return new Furniture()
            {
                register = register,
                value = val,
            };
        }

        // --- Allocation-Free Duck-Typed Foreach Structs ---
        public struct FurnitureEnumerable
        {
            private readonly List<FurnitureElement> _pool;
            private readonly int _headIndex;
            public int Length
            {
                get { return _pool.Count; }
            }

            public FurnitureEnumerable(List<FurnitureElement> pool, int headIndex)
            {
                _pool = pool;
                _headIndex = headIndex;
            }

            public FurnitureEnumerator GetEnumerator() => new FurnitureEnumerator(_pool, _headIndex);
        }

        public struct FurnitureEnumerator
        {
            private readonly List<FurnitureElement> _pool;
            private int _currentIndex;

            public FurnitureEnumerator(List<FurnitureElement> pool, int headIndex)
            {
                _pool = pool;
                _currentIndex = headIndex;
                Current = default;
            }

            public bool MoveNext()
            {
                if (_pool == null || _currentIndex == -1 || _currentIndex >= _pool.Count)
                    return false;

                Current = _pool[_currentIndex].furniture;
                _currentIndex = _pool[_currentIndex].nextIndex;
                return true;
            }

            public Furniture Current { get; private set; }
        }
    }
}