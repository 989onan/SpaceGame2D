using SpaceGame2D.enviroment.physics;
using SpaceGame2D.utilities.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace SpaceGame2D.utilities.threading
{

    //Thanks to Omni's Hackpad for this!
    public class Octree2D<T> where T: ICollideable
    {
        public int resolution;

        private static readonly int _BucketCapacity = 32;
        private static readonly int _MaxDepth = 12;

        public bool IsLeaf => _upperLeft == null && _upperRight == null && _lowerLeft == null && _lowerRight == null;

        private int Level = 0;

        public List<T> _elements = new List<T>(_BucketCapacity); //so that we don't have to do expansion internally all the time unless needed so it's a little faster

        public AABB bounds;
        # nullable enable
        private Octree2D<T>? _upperLeft;
        private Octree2D<T>? _upperRight;
        private Octree2D<T>? _lowerLeft;
        private Octree2D<T>? _lowerRight;


        public Octree2D()
        {
            this.bounds = new AABB(0, 0, 1, 1);
        }

        public Octree2D(AABB bounds) {
            this.bounds = bounds;
        }

        
        public Octree2D(AABB bounds, int Level)
        {
            this.bounds = bounds;
            this.Level = Level;
        }

        public void Add(T obj)
        {
            if(obj == null) throw new ArgumentNullException(nameof(obj));
            if (!this.bounds.ContainsFully(obj.Collider))
            {
                Console.WriteLine("Cannot add object to physics tree because it is outside of the world size! Discarding!!!");
                Console.WriteLine(obj.Collider.ToString());
            }

            if(this._elements.Count() >= _BucketCapacity)
            {
                Split();
            }

            Octree2D<T>? ContainingChild = GetContainingChild(obj.Collider);

            if (ContainingChild != null)
            {
                ContainingChild.Add(obj);
            }
            else
            {
                //Console.WriteLine("octree has "+_elements.Count().ToString()+" elements in it's bucket.");
                _elements.Add(obj);
            }
        }

        public Octree2D<T>? GetContainingChild(AABB collider)
        {
            if(_upperLeft != null)
            {
                //Console.WriteLine("upper left: "+_upperLeft.bounds.ToString());
                if (_upperLeft.bounds.ContainsFully(collider))
                {
                    return _upperLeft;
                }
            }
            if (_upperRight != null)
            {
                //Console.WriteLine(_upperRight.bounds.ToString());
                if (_upperRight.bounds.ContainsFully(collider))
                {
                    return _upperRight;
                }
            }
            if (_lowerLeft != null)
            {
                //Console.WriteLine(_lowerLeft.bounds.ToString());
                if (_lowerLeft.bounds.ContainsFully(collider))
                {
                    return _lowerLeft;
                }
            }
            if (_lowerRight != null)
            {
                //Console.WriteLine(_lowerRight.bounds.ToString());
                if (_lowerRight.bounds.ContainsFully(collider))
                {
                    return _lowerRight;
                }
            }
            return null;
        }

        private void Split()
        {
            if (!IsLeaf) return;
            if (Level + 1 > _MaxDepth) return;
            //Console.WriteLine("splitting octree!");
            _lowerRight = new Octree2D<T>(new AABB(this.bounds.Center, this.bounds.Maximum), Level + 1);
            _upperLeft = new Octree2D<T>(new AABB(this.bounds.Minimum, this.bounds.Center), Level + 1);
            
            _lowerLeft = new Octree2D<T>(new AABB(new Vector2(this.bounds.Center.Y,this.bounds.Minimum.X), new Vector2(this.bounds.Center.X, this.bounds.Maximum.Y)), Level+1);
            _upperRight = new Octree2D<T>(new AABB(new Vector2(this.bounds.Center.X, this.bounds.Minimum.Y), new Vector2(this.bounds.Maximum.X, this.bounds.Center.Y)), Level + 1);
            T[] old_elements = this._elements.ToArray();
            foreach (T item in old_elements)
            {
                
                Octree2D<T>? containingChild = GetContainingChild(item.Collider);
                //Console.WriteLine((containingChild != null).ToString());
                if (containingChild != null)
                {
                    _elements.Remove(item).ToString();

                    containingChild.Add(item);
                }

            }
        }

        private int CountElements()
        {
            if(this.IsLeaf) return _elements.Count();
            else
            {
                int count = 0;
                if(_upperLeft != null)
                {
                    count += _upperLeft.CountElements();
                }
                if (_upperRight != null)
                {
                    count += _upperRight.CountElements();
                }
                if (_lowerLeft != null)
                {
                    count += _lowerLeft.CountElements();
                }
                if (_lowerRight != null)
                {
                    count += _lowerRight.CountElements();
                }
                return count;
            }

            


        }

        public bool Remove(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Octree2D<T>? containingChild = GetContainingChild(obj.Collider);

            bool removed = containingChild?.Remove(obj) ?? _elements.Remove(obj);

            if(removed && CountElements() <= _BucketCapacity)
            {
                Merge();
            }

            return removed;
        }

        private void Merge()
        {
            if (IsLeaf) return;
            if(_upperLeft != null)
            {
                _elements.AddRange(_upperLeft._elements);
            }
            if (_upperRight != null)
            {
                _elements.AddRange(_upperRight._elements);
            }
            if (_lowerLeft != null)
            {
                _elements.AddRange(_lowerLeft._elements);
            }
            if (_lowerRight != null)
            {
                _elements.AddRange(_lowerRight._elements);
            }
            _upperLeft = _upperRight = _lowerLeft = _lowerRight = null;
        }


        public IEnumerable<T> FindCollisions(AABB collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));

            Queue<Octree2D<T>> nodes = new Queue<Octree2D<T>>();
            List<T> result = new List<T>();


            nodes.Enqueue(this);

            while ( nodes.Count > 0)
            {
                Octree2D<T> node = nodes.Dequeue();
                if (!collider.Intercects(node.bounds)) continue;
                //Console.WriteLine("finding collisions");
                result.AddRange(node._elements.Where(e => e.Collider.Intercects(collider)));
                if (!node.IsLeaf)
                {
                    if (node._upperLeft != null)
                    {
                        if (node._upperLeft.bounds.Intercects(collider))
                        {
                            nodes.Enqueue(node._upperLeft);
                        }
                    }
                    if (node._upperRight != null)
                    {
                        if (node._upperRight.bounds.Intercects(collider))
                        {
                            nodes.Enqueue(node._upperRight);
                        }
                    }
                    if (node._lowerLeft != null)
                    {
                        if (node._lowerLeft.bounds.Intercects(collider))
                        {
                            nodes.Enqueue(node._lowerLeft);
                            //Console.WriteLine("found lower left");
                        }
                    }
                    if (node._lowerRight != null)
                    {
                        if (node._lowerRight.bounds.Intercects(collider))
                        {
                            nodes.Enqueue(node._lowerRight);
                            //Console.WriteLine("found lower right");
                        }
                    }
                }
            }


            return result;
        }
    }
    #nullable disable

}
