using System;
using System.Linq;
using Game.Utils;
using Godot;
using Godot.Collections;

namespace Game
{
    public partial class ResourcesSaver
    {
        public string TmpFile = "user://tmp/tmp";

        public string ConvertResourceToString<[MustBeVariant] T>(T resVariant, string tag)
        {
            var res = Variant.From(resVariant).As<Resource>();

            var packedResource = "";

            var fileType = ".tres";
            if (res is PackedScene) fileType = ".tscn";
            var tmpItemResPath = TmpFile + fileType;
            var result = ResourceSaver.Save(res, tmpItemResPath);
            if (result == Error.Ok)
            {
                var file = FileAccess.Open(tmpItemResPath, FileAccess.ModeFlags.Read);
                var text = file.GetAsText();
                file.Close();
                packedResource = text;
            }

            return packedResource;
        }


        // DirAccess.MakeDirRecursiveAbsolute(SavesDirectory);
    }
}