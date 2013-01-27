using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LinguaSpace.Common.ComponentModel
{
    public interface IBinarySerializable
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}
