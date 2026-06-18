using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Node_Related_Scripts.ConnectionStuff
{
    [Serializable]
    public class ConnectionType
    {
        [Serializable]
        public class VirtualPort : IEquatable<VirtualPort>
        {

            public static readonly VirtualPort Null = new VirtualPort()
            {
                connectedNode = -1,
                portIndex = -1,
            };
            public int portIndex;
            public int connectedNode;
            public override int GetHashCode()
            {
                unchecked // Prevents throwing overflow exceptions in arithmetic-checking environments
                {
                    int hash = 17;
                    hash = hash * 23 + portIndex.GetHashCode();
                    hash = hash * 23 + connectedNode.GetHashCode();
                    return hash;
                }
            }

            // 2. Strongly-typed Equals (implements IEquatable<VirtualPort>)
            // This avoids boxing performance penalties
            public bool Equals(VirtualPort other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return portIndex == other.portIndex && connectedNode == other.connectedNode;
            }

            // 3. Fallback Equals for general objects
            public override bool Equals(object obj)
            {
                return Equals(obj as VirtualPort);
            }

            // 4. Operator overloads so you can use == and !=
            public static bool operator ==(VirtualPort left, VirtualPort right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(VirtualPort left, VirtualPort right)
            {
                return !(left == right);
            }
        }
        public VirtualPort first;
        public VirtualPort second;
        public NodeBlueprint.ConnectionClass connectionClass;
    }
}
