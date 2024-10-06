using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SpaceGame2D.enviroment.physics;
using System.Data.Common;
using SpaceGame2D.enviroment.blocks;
using System.Drawing;

namespace SpaceGame2D.utilities.math
{
    public class AABBVoxel
    {
        public List<IStaticPhysicsObject> whatMakesMeUp = new List<IStaticPhysicsObject>();

        public static readonly int chunking_max = 100;

        public AABB generalization;


        private static bool get_1D(int width, int x, int y, bool[] bools)
        {
            return bools[x + (y * width)];
        }

        private static void set_1D(int width, int x, int y, bool[] bools, bool val)
        {
            bools[x + (y * width)] = val;
        }

        private static Tuple<int,int> get_2D_pos(int width, int height, int pos)
        {
           return new Tuple<int,int>(pos % width, (int)((float)pos / (float)width));
        }

        private class ModifiableTuple<T, T1>
        {
            public T Item1;
            public T1 Item2;

            public ModifiableTuple(T point1, T1 point2)
            {
                this.Item1 = point1;
                this.Item2 = point2;
            }
            public override string ToString()
            {
                return "Item1: " + Item1.ToString() + " Item2: " + Item2.ToString();
            }
        }


        //this is the simplest I could do it.
        public static List<AABBVoxel> VoxelizeGrid(BlockGrid blockGrid)
        {

            List<AABBVoxel> voxels = new List<AABBVoxel>();

            //x,y,hasblock
            bool[] has_block = new bool[blockGrid.size.X * blockGrid.size.Y];


            List<ModifiableTuple<Point, Point>> AABBs = new List<ModifiableTuple<Point, Point>>();



            for (int y = 0; y < blockGrid.size.Y; y++)
            {

                //Console.WriteLine();
                for (int x = 0; x < blockGrid.size.X; x++)
                {

                    IBlock block = blockGrid.getTileAt(x, y);
                    if (block == null)
                    {
                        set_1D(blockGrid.size.X, x, y, has_block, false);
                        //Console.Write(get_1D(blockGrid.size.X,x, y, has_block) ? "1" : "0");
                        continue;
                    }
                    if (!block.HasCollision)
                    {
                        set_1D(blockGrid.size.X, x, y, has_block, false);
                        //Console.Write(get_1D(blockGrid.size.X, x, y, has_block) ? "1" : "0");
                        continue;
                    }


                    set_1D(blockGrid.size.X, x, y, has_block, true);

                    //Console.Write(get_1D(blockGrid.size.X, x, y, has_block) ? "1" : "0");


                }

            }

            int pos = 0;
            while(has_block.Any(o => o == true))
            {
                if (has_block[pos])
                {
                    Tuple<int, int> result = get_2D_pos(blockGrid.size.X, blockGrid.size.Y, pos);
                    int x = result.Item1;
                    int y = result.Item2;


                    int end_x = x;
                    int end_y = y;
                    bool alivex = true;
                    bool alivey = true;
                    ModifiableTuple<Point, Point> AABB = new ModifiableTuple<Point, Point>(new Point(x, y), new Point(end_x, end_y));
                    //expand in both directions till we're too big.

                    //testing here is what this is.
                    /*for (int i = 1; i <= 1; i++)
                    {
                        Console.WriteLine("fire!");
                    }*/

                    bool stop_x = false;
                    bool stop_y = false;
                    //Console.WriteLine("starting expansion loop!");
                    while (!stop_x || !stop_y)
                    {
                        
                        alivex = (end_x - x) < (int)Math.Sqrt(chunking_max) && end_x < blockGrid.size.X && !stop_x;
                        if (alivex)
                        {
                            //Console.WriteLine(((end_x - x)).ToString());
                            //Console.WriteLine("y: "+y.ToString()+"end_y: "+end_y.ToString());
                            for (int y_iter = 0; y_iter <= (end_y-y); y_iter++)
                            {

                                if (!get_1D(blockGrid.size.X, end_x, y+y_iter, has_block))
                                {
                                    stop_x = true;
                                    //end_x--;
                                    break;
                                }

                            }
                        }
                        else
                        {
                           // Console.WriteLine("stuck x!");
                            stop_x = true;
                            end_x--;
                            AABB.Item2.X = end_x;
                        }

                        alivey = (end_y - y) < (int)Math.Sqrt(chunking_max) && end_y < blockGrid.size.Y && !stop_y;
                        if (alivey)
                        {
                            //Console.WriteLine("x: " + x.ToString() + "end_x: " + end_x.ToString());
                            //Console.WriteLine(((end_y - y)).ToString());
                            for (int x_iter = 0; x_iter <= (end_x-x); x_iter++)
                            {

                                if (!get_1D(blockGrid.size.X, x+x_iter, end_y, has_block))
                                {
                                    stop_y = true;
                                    //end_y--;
                                    break;
                                }

                            }

                        }
                        else
                        {
                            //Console.WriteLine("stuck y!");
                            stop_y = true;
                            end_y--;
                            AABB.Item2.Y = end_y;
                        }

                        if (alivex && !stop_x)
                        {
                            end_x++;

                        }
                        if (alivey && !stop_y)
                        {
                            end_y++;

                        }



                    }

                    AABBVoxel final_voxel_collect = new AABBVoxel();
                    //Console.WriteLine(AABB.ToString());
                    for (int x_final = AABB.Item1.X; x_final <= AABB.Item2.X; x_final++)
                    {
                        for (int y_final = AABB.Item1.Y; y_final <= AABB.Item2.Y; y_final++)
                        {
                            //try
                            //{
                                set_1D(blockGrid.size.X, x_final, y_final, has_block, false);
                                final_voxel_collect.Add(blockGrid.getTileAt(x_final, y_final));
                            /*}
                            catch (Exception e)
                            {
                                 ("blockGrid does not have a tile at x: " + x_final.ToString() + "y: " + y_final.ToString());

                            }*/
                        }
                        voxels.Add(final_voxel_collect);
                    }
                }
                pos++;
                if (pos >= has_block.Length)
                {
                    pos = 0;
                }
            }


            Console.WriteLine("Generated voxels for BlockGrid. voxel count is: " + voxels.Count().ToString());


            return voxels;
        }



        public void TriggerCollideEvent(IActivePhysicsObject physicsObject, Vector2 normal)
        {

        }

        public bool isEmpty => this.whatMakesMeUp.Select(o=>o!= null && o.HasCollision).Any();

        public AABBVoxel() {

            generalization = new AABB();



        }




        public AABBVoxel(AABBVoxel original)
        {
            whatMakesMeUp.AddRange(original.whatMakesMeUp);
            generalization = original.generalization;
        }


        private void recalculate()
        {
            generalization = new AABB();
            foreach (IStaticPhysicsObject item in whatMakesMeUp)
            {
                generalization.EncapsulateBounds(item.Collider);
            }


        }

        public void Add(IStaticPhysicsObject aabb)
        {
            if(this.whatMakesMeUp.Count()+1 > chunking_max)
            {
                throw new IndexOutOfRangeException("The voxel has reached max capacity! This should not happen!");
            }
            this.whatMakesMeUp.Add(aabb);
            if (generalization.size.X == 0 && generalization.size.Y == 0)
            {
                generalization = new AABB(aabb.Collider);
            }
            generalization.EncapsulateBounds(aabb.Collider);
        }


        public void Remove(IStaticPhysicsObject aabb)
        {
            
            this.whatMakesMeUp.Remove(aabb);
            this.recalculate();
        }
    }
}
