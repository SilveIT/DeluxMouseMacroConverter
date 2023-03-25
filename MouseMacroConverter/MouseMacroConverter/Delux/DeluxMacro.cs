using System.Collections.Generic;
using BinarySilvelizerX.Attributes;
using BinarySilvelizerX.Core;
using BinarySilvelizerX.Entities;
using BinarySilvelizerX.Utils;

namespace MouseMacroConverter.Delux
{
    public class DeluxMacro : ByteModel<DeluxMacro>
    {
        public int Key { get; set; }
        public int WeirdTime { get; set; }
        public int Unk0 { get; set; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        [BFLength(38)]
        [BFEncoding(TextUtils.CodePage.Utf16)]
        public string Name { get; set; }
        public ushort Unk3 { get; set; }
        public int Unk4 { get; set; }
        public ushort Unk5 { get; set; }
        [BFLength(168)]
        public List<DeluxInstruction> Instructions { get; set; }
        [BFSpacer(30)]
        public ByteSpacer EndSpacer { get; set; }
    }

    public class DeluxInstruction : ByteModel<DeluxInstruction>
    {
        public byte Action { get; set; }
        public byte Param { get; set; }
        public ushort Pressed { get; set; }
        public uint DelayAfter { get; set; }
    }
}
