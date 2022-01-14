using System;
using fNbt;
using VoxReader;
using VoxReader.Interfaces;

namespace VoxToStructure.format.schematic
{
    public class SchematicFile
    {
        private Int16 Width;
        private Int16 Height;
        private Int16 Length;
        
        public NbtFile FromVox(IVoxFile voxFile)
        {
            // Access models of .vox file
            IModel[] models = voxFile.Models;
            // Access voxels of first model in the file
            Voxel[] voxels = models[0].Voxels;
            Vector3 size = models[0].Size;
            Width = (Int16)size.X;
            Height = (Int16)size.Y;
            Length = (Int16)size.Z;

            byte[] blocks = new byte[Width * Height * Length];

            foreach (var voxel in voxels)
            {
                // Access properties of a voxel
                Vector3 position = voxel.Position;
                //Color color = voxels[0].Color;
                blocks[BlockIndex(position.X, position.Y, position.Z)] = 1;//all stone for now TODO add blockid based on color
            }
            
            var rootTag = new NbtCompound("root");
            rootTag.Add(new NbtShort("Width", Width));
            rootTag.Add(new NbtShort("Height", Height));
            rootTag.Add(new NbtShort("Length", Length));
            rootTag.Add(new NbtString("Materials", "Alpha"));//Pocket
            rootTag.Add(new NbtByteArray("Blocks", blocks));
            rootTag.Add(new NbtByteArray("Data", new byte[Width * Height * Length]));
            rootTag.Add(new NbtList("Entities", NbtTagType.Compound));
            rootTag.Add(new NbtList("TileEntities", NbtTagType.Compound));
            return new NbtFile(rootTag);
        }

        public void Save(NbtFile file, string outputPath)
        {
            file.SaveToFile(outputPath, NbtCompression.GZip);
        }
        
        private int BlockIndex(int x, int y, int z){
            //return (y * Length + z) * Width + x;
            return (z * Length + y) * Width + x;
        }
    }
}