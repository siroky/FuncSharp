﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FuncSharp
{
    /// <summary>
    /// A 0-dimensional data cube.
    /// </summary>
    public class DataCube0<TValue> : DataCube<IProduct0, TValue>
    {
        /// <summary>
        /// Creates an empty 0-dimensional data cube. 
        /// </summary>
        public DataCube0()
        {
        }

        /// <summary>
        /// Creates a 0-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube0(IEnumerable<IProduct2<IProduct0, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ProductValue1, d.ProductValue2);
            }
        }

        /// <summary>
        /// The only value in the cube.
        /// </summary>
        public IOption<TValue> Value 
        { 
            get { return Get(); }
        }

        /// <summary>
        /// For each value in the cube, invokes the specified function passing in the position and the stored value.
        /// </summary>
        public void ForEach(Action<TValue> a)
        {
            ForEach((position, value) => a(value));
        }

        /// <summary>
        /// Returns whether the cube contains a value at the specified position.
        /// </summary>
        public bool Contains()
        {
            return Contains(Product.Create());
        }

        /// <summary>
        /// Returns value at the specified position.
        /// </summary>
        public IOption<TValue> Get()
        {
            return Get(Product.Create());
        }

        /// <summary>
        /// Returns value at the specified position. If there is no value present, sets the position to value generated by 
        /// the <paramref name="setter"/> function and returns the newly generated value.
        /// </summary>
        public TValue GetOrElseSet(Func<TValue> setter)
        {
            return GetOrElseSet(Product.Create(), setter);
        }
        
        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public override TValue Set(IProduct0 position, TValue value)
        {
            return base.Set(position, value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public TValue Set(TValue value)
        {
            return Set(Product.Create(), value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, updates it with the
        /// result of the <paramref name="updater"/> function which is given the present value and the new value.
        /// </summary>
        public TValue SetOrElseUpdate(TValue value, Func<TValue, TValue, TValue> updater)
        {
            return SetOrElseUpdate(Product.Create(), value, updater);
        }
    }

    /// <summary>
    /// A 1-dimensional data cube.
    /// </summary>
    public class DataCube1<P1, TValue> : DataCube<IProduct1<P1>, TValue>
    {
        /// <summary>
        /// Creates an empty 1-dimensional data cube. 
        /// </summary>
        public DataCube1()
        {
            Domain1Counts = new Dictionary<IProduct1<P1>, int>();
        }

        /// <summary>
        /// Creates a 1-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube1(IEnumerable<IProduct2<IProduct1<P1>, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ProductValue1, d.ProductValue2);
            }
        }

        /// <summary>
        /// Creates a 1-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube1(IEnumerable<IProduct2<P1, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ExceptValue2, d.ProductValue2);
            }
        }

        /// <summary>
        /// Positions of values in the first dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P1> Domain1
        {
            get { return Domain1Counts.Keys.Select(p => p.ProductValue1); }
        }

        private Dictionary<IProduct1<P1>, int> Domain1Counts { get; set; }

        /// <summary>
        /// For each value in the cube, invokes the specified function passing in the position and the stored value.
        /// </summary>
        public void ForEach(Action<P1, TValue> a)
        {
            ForEach((position, value) => a(position.ProductValue1, value));
        }

        /// <summary>
        /// Returns whether the cube contains a value at the specified position.
        /// </summary>
        public bool Contains(P1 p1)
        {
            return Contains(Product.Create(p1));
        }

        /// <summary>
        /// Returns value at the specified position.
        /// </summary>
        public IOption<TValue> Get(P1 p1)
        {
            return Get(Product.Create(p1));
        }

        /// <summary>
        /// Returns value at the specified position. If there is no value present, sets the position to value generated by 
        /// the <paramref name="setter"/> function and returns the newly generated value.
        /// </summary>
        public TValue GetOrElseSet(P1 p1, Func<TValue> setter)
        {
            return GetOrElseSet(Product.Create(p1), setter);
        }
        
        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public override TValue Set(IProduct1<P1> position, TValue value)
        {
            AddDomain(Domain1Counts, position.ProductValue1);
            return base.Set(position, value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public TValue Set(P1 p1, TValue value)
        {
            return Set(Product.Create(p1), value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, updates it with the
        /// result of the <paramref name="updater"/> function which is given the present value and the new value.
        /// </summary>
        public TValue SetOrElseUpdate(P1 p1, TValue value, Func<TValue, TValue, TValue> updater)
        {
            return SetOrElseUpdate(Product.Create(p1), value, updater);
        }

        /// <summary>
        /// Transforms the current 1-dimensional cube into a 0-dimensional cube by excluding the dimension 1.
        /// All values whose position differ just in dimension 1 (their positions without dimension 1 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 1.
        /// </summary>
        public DataCube0<TValue> RollUpDimension1(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube0<TValue>, IProduct0>(
                position => position.ExceptValue1, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 1. The slices are 0-dimensional cubes without dimension 1 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P1, DataCube0<TValue>> SliceDimension1()
        {
            var slices = new DataCube1<P1, DataCube0<TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue1, () => new DataCube0<TValue>());
                slice.Set(position.ExceptValue1, value);
            });
            return slices;
        }
    }

    /// <summary>
    /// A 2-dimensional data cube.
    /// </summary>
    public class DataCube2<P1, P2, TValue> : DataCube<IProduct2<P1, P2>, TValue>
    {
        /// <summary>
        /// Creates an empty 2-dimensional data cube. 
        /// </summary>
        public DataCube2()
        {
            Domain1Counts = new Dictionary<IProduct1<P1>, int>();
            Domain2Counts = new Dictionary<IProduct1<P2>, int>();
        }

        /// <summary>
        /// Creates a 2-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube2(IEnumerable<IProduct2<IProduct2<P1, P2>, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ProductValue1, d.ProductValue2);
            }
        }

        /// <summary>
        /// Creates a 2-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube2(IEnumerable<IProduct3<P1, P2, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ExceptValue3, d.ProductValue3);
            }
        }

        /// <summary>
        /// Positions of values in the first dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P1> Domain1
        {
            get { return Domain1Counts.Keys.Select(p => p.ProductValue1); }
        }

        /// <summary>
        /// Positions of values in the second dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P2> Domain2
        {
            get { return Domain2Counts.Keys.Select(p => p.ProductValue1); }
        }

        private Dictionary<IProduct1<P1>, int> Domain1Counts { get; set; }

        private Dictionary<IProduct1<P2>, int> Domain2Counts { get; set; }

        /// <summary>
        /// For each value in the cube, invokes the specified function passing in the position and the stored value.
        /// </summary>
        public void ForEach(Action<P1, P2, TValue> a)
        {
            ForEach((position, value) => a(position.ProductValue1, position.ProductValue2, value));
        }

        /// <summary>
        /// Returns whether the cube contains a value at the specified position.
        /// </summary>
        public bool Contains(P1 p1, P2 p2)
        {
            return Contains(Product.Create(p1, p2));
        }

        /// <summary>
        /// Returns value at the specified position.
        /// </summary>
        public IOption<TValue> Get(P1 p1, P2 p2)
        {
            return Get(Product.Create(p1, p2));
        }

        /// <summary>
        /// Returns value at the specified position. If there is no value present, sets the position to value generated by 
        /// the <paramref name="setter"/> function and returns the newly generated value.
        /// </summary>
        public TValue GetOrElseSet(P1 p1, P2 p2, Func<TValue> setter)
        {
            return GetOrElseSet(Product.Create(p1, p2), setter);
        }
        
        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public override TValue Set(IProduct2<P1, P2> position, TValue value)
        {
            AddDomain(Domain1Counts, position.ProductValue1);
            AddDomain(Domain2Counts, position.ProductValue2);
            return base.Set(position, value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public TValue Set(P1 p1, P2 p2, TValue value)
        {
            return Set(Product.Create(p1, p2), value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, updates it with the
        /// result of the <paramref name="updater"/> function which is given the present value and the new value.
        /// </summary>
        public TValue SetOrElseUpdate(P1 p1, P2 p2, TValue value, Func<TValue, TValue, TValue> updater)
        {
            return SetOrElseUpdate(Product.Create(p1, p2), value, updater);
        }

        /// <summary>
        /// Transforms the current 2-dimensional cube into a 1-dimensional cube by excluding the dimension 1.
        /// All values whose position differ just in dimension 1 (their positions without dimension 1 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 1.
        /// </summary>
        public DataCube1<P2, TValue> RollUpDimension1(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube1<P2, TValue>, IProduct1<P2>>(
                position => position.ExceptValue1, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 1. The slices are 1-dimensional cubes without dimension 1 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P1, DataCube1<P2, TValue>> SliceDimension1()
        {
            var slices = new DataCube1<P1, DataCube1<P2, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue1, () => new DataCube1<P2, TValue>());
                slice.Set(position.ExceptValue1, value);
            });
            return slices;
        }

        /// <summary>
        /// Transforms the current 2-dimensional cube into a 1-dimensional cube by excluding the dimension 2.
        /// All values whose position differ just in dimension 2 (their positions without dimension 2 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 2.
        /// </summary>
        public DataCube1<P1, TValue> RollUpDimension2(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube1<P1, TValue>, IProduct1<P1>>(
                position => position.ExceptValue2, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 2. The slices are 1-dimensional cubes without dimension 2 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P2, DataCube1<P1, TValue>> SliceDimension2()
        {
            var slices = new DataCube1<P2, DataCube1<P1, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue2, () => new DataCube1<P1, TValue>());
                slice.Set(position.ExceptValue2, value);
            });
            return slices;
        }
    }

    /// <summary>
    /// A 3-dimensional data cube.
    /// </summary>
    public class DataCube3<P1, P2, P3, TValue> : DataCube<IProduct3<P1, P2, P3>, TValue>
    {
        /// <summary>
        /// Creates an empty 3-dimensional data cube. 
        /// </summary>
        public DataCube3()
        {
            Domain1Counts = new Dictionary<IProduct1<P1>, int>();
            Domain2Counts = new Dictionary<IProduct1<P2>, int>();
            Domain3Counts = new Dictionary<IProduct1<P3>, int>();
        }

        /// <summary>
        /// Creates a 3-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube3(IEnumerable<IProduct2<IProduct3<P1, P2, P3>, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ProductValue1, d.ProductValue2);
            }
        }

        /// <summary>
        /// Creates a 3-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube3(IEnumerable<IProduct4<P1, P2, P3, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ExceptValue4, d.ProductValue4);
            }
        }

        /// <summary>
        /// Positions of values in the first dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P1> Domain1
        {
            get { return Domain1Counts.Keys.Select(p => p.ProductValue1); }
        }

        /// <summary>
        /// Positions of values in the second dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P2> Domain2
        {
            get { return Domain2Counts.Keys.Select(p => p.ProductValue1); }
        }

        /// <summary>
        /// Positions of values in the third dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P3> Domain3
        {
            get { return Domain3Counts.Keys.Select(p => p.ProductValue1); }
        }

        private Dictionary<IProduct1<P1>, int> Domain1Counts { get; set; }

        private Dictionary<IProduct1<P2>, int> Domain2Counts { get; set; }

        private Dictionary<IProduct1<P3>, int> Domain3Counts { get; set; }

        /// <summary>
        /// For each value in the cube, invokes the specified function passing in the position and the stored value.
        /// </summary>
        public void ForEach(Action<P1, P2, P3, TValue> a)
        {
            ForEach((position, value) => a(position.ProductValue1, position.ProductValue2, position.ProductValue3, value));
        }

        /// <summary>
        /// Returns whether the cube contains a value at the specified position.
        /// </summary>
        public bool Contains(P1 p1, P2 p2, P3 p3)
        {
            return Contains(Product.Create(p1, p2, p3));
        }

        /// <summary>
        /// Returns value at the specified position.
        /// </summary>
        public IOption<TValue> Get(P1 p1, P2 p2, P3 p3)
        {
            return Get(Product.Create(p1, p2, p3));
        }

        /// <summary>
        /// Returns value at the specified position. If there is no value present, sets the position to value generated by 
        /// the <paramref name="setter"/> function and returns the newly generated value.
        /// </summary>
        public TValue GetOrElseSet(P1 p1, P2 p2, P3 p3, Func<TValue> setter)
        {
            return GetOrElseSet(Product.Create(p1, p2, p3), setter);
        }
        
        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public override TValue Set(IProduct3<P1, P2, P3> position, TValue value)
        {
            AddDomain(Domain1Counts, position.ProductValue1);
            AddDomain(Domain2Counts, position.ProductValue2);
            AddDomain(Domain3Counts, position.ProductValue3);
            return base.Set(position, value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public TValue Set(P1 p1, P2 p2, P3 p3, TValue value)
        {
            return Set(Product.Create(p1, p2, p3), value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, updates it with the
        /// result of the <paramref name="updater"/> function which is given the present value and the new value.
        /// </summary>
        public TValue SetOrElseUpdate(P1 p1, P2 p2, P3 p3, TValue value, Func<TValue, TValue, TValue> updater)
        {
            return SetOrElseUpdate(Product.Create(p1, p2, p3), value, updater);
        }

        /// <summary>
        /// Transforms the current 3-dimensional cube into a 2-dimensional cube by excluding the dimension 1.
        /// All values whose position differ just in dimension 1 (their positions without dimension 1 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 1.
        /// </summary>
        public DataCube2<P2, P3, TValue> RollUpDimension1(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube2<P2, P3, TValue>, IProduct2<P2, P3>>(
                position => position.ExceptValue1, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 1. The slices are 2-dimensional cubes without dimension 1 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P1, DataCube2<P2, P3, TValue>> SliceDimension1()
        {
            var slices = new DataCube1<P1, DataCube2<P2, P3, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue1, () => new DataCube2<P2, P3, TValue>());
                slice.Set(position.ExceptValue1, value);
            });
            return slices;
        }

        /// <summary>
        /// Transforms the current 3-dimensional cube into a 2-dimensional cube by excluding the dimension 2.
        /// All values whose position differ just in dimension 2 (their positions without dimension 2 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 2.
        /// </summary>
        public DataCube2<P1, P3, TValue> RollUpDimension2(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube2<P1, P3, TValue>, IProduct2<P1, P3>>(
                position => position.ExceptValue2, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 2. The slices are 2-dimensional cubes without dimension 2 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P2, DataCube2<P1, P3, TValue>> SliceDimension2()
        {
            var slices = new DataCube1<P2, DataCube2<P1, P3, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue2, () => new DataCube2<P1, P3, TValue>());
                slice.Set(position.ExceptValue2, value);
            });
            return slices;
        }

        /// <summary>
        /// Transforms the current 3-dimensional cube into a 2-dimensional cube by excluding the dimension 3.
        /// All values whose position differ just in dimension 3 (their positions without dimension 3 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 3.
        /// </summary>
        public DataCube2<P1, P2, TValue> RollUpDimension3(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube2<P1, P2, TValue>, IProduct2<P1, P2>>(
                position => position.ExceptValue3, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 3. The slices are 2-dimensional cubes without dimension 3 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P3, DataCube2<P1, P2, TValue>> SliceDimension3()
        {
            var slices = new DataCube1<P3, DataCube2<P1, P2, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue3, () => new DataCube2<P1, P2, TValue>());
                slice.Set(position.ExceptValue3, value);
            });
            return slices;
        }
    }

    /// <summary>
    /// A 4-dimensional data cube.
    /// </summary>
    public class DataCube4<P1, P2, P3, P4, TValue> : DataCube<IProduct4<P1, P2, P3, P4>, TValue>
    {
        /// <summary>
        /// Creates an empty 4-dimensional data cube. 
        /// </summary>
        public DataCube4()
        {
            Domain1Counts = new Dictionary<IProduct1<P1>, int>();
            Domain2Counts = new Dictionary<IProduct1<P2>, int>();
            Domain3Counts = new Dictionary<IProduct1<P3>, int>();
            Domain4Counts = new Dictionary<IProduct1<P4>, int>();
        }

        /// <summary>
        /// Creates a 4-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube4(IEnumerable<IProduct2<IProduct4<P1, P2, P3, P4>, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ProductValue1, d.ProductValue2);
            }
        }

        /// <summary>
        /// Creates a 4-dimensional data cube filled with the specified data. If multiple values with the same position
        /// appear among the initial data, last one is used.
        /// </summary>
        public DataCube4(IEnumerable<IProduct5<P1, P2, P3, P4, TValue>> initialData)
            : this()
        {
            foreach (var d in initialData)
            {
                Set(d.ExceptValue5, d.ProductValue5);
            }
        }

        /// <summary>
        /// Positions of values in the first dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P1> Domain1
        {
            get { return Domain1Counts.Keys.Select(p => p.ProductValue1); }
        }

        /// <summary>
        /// Positions of values in the second dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P2> Domain2
        {
            get { return Domain2Counts.Keys.Select(p => p.ProductValue1); }
        }

        /// <summary>
        /// Positions of values in the third dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P3> Domain3
        {
            get { return Domain3Counts.Keys.Select(p => p.ProductValue1); }
        }

        /// <summary>
        /// Positions of values in the fourth dimension (domain of that dimension).
        /// </summary>
        public IEnumerable<P4> Domain4
        {
            get { return Domain4Counts.Keys.Select(p => p.ProductValue1); }
        }

        private Dictionary<IProduct1<P1>, int> Domain1Counts { get; set; }

        private Dictionary<IProduct1<P2>, int> Domain2Counts { get; set; }

        private Dictionary<IProduct1<P3>, int> Domain3Counts { get; set; }

        private Dictionary<IProduct1<P4>, int> Domain4Counts { get; set; }

        /// <summary>
        /// For each value in the cube, invokes the specified function passing in the position and the stored value.
        /// </summary>
        public void ForEach(Action<P1, P2, P3, P4, TValue> a)
        {
            ForEach((position, value) => a(position.ProductValue1, position.ProductValue2, position.ProductValue3, position.ProductValue4, value));
        }

        /// <summary>
        /// Returns whether the cube contains a value at the specified position.
        /// </summary>
        public bool Contains(P1 p1, P2 p2, P3 p3, P4 p4)
        {
            return Contains(Product.Create(p1, p2, p3, p4));
        }

        /// <summary>
        /// Returns value at the specified position.
        /// </summary>
        public IOption<TValue> Get(P1 p1, P2 p2, P3 p3, P4 p4)
        {
            return Get(Product.Create(p1, p2, p3, p4));
        }

        /// <summary>
        /// Returns value at the specified position. If there is no value present, sets the position to value generated by 
        /// the <paramref name="setter"/> function and returns the newly generated value.
        /// </summary>
        public TValue GetOrElseSet(P1 p1, P2 p2, P3 p3, P4 p4, Func<TValue> setter)
        {
            return GetOrElseSet(Product.Create(p1, p2, p3, p4), setter);
        }
        
        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public override TValue Set(IProduct4<P1, P2, P3, P4> position, TValue value)
        {
            AddDomain(Domain1Counts, position.ProductValue1);
            AddDomain(Domain2Counts, position.ProductValue2);
            AddDomain(Domain3Counts, position.ProductValue3);
            AddDomain(Domain4Counts, position.ProductValue4);
            return base.Set(position, value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, overwrites it.
        /// </summary>
        public TValue Set(P1 p1, P2 p2, P3 p3, P4 p4, TValue value)
        {
            return Set(Product.Create(p1, p2, p3, p4), value);
        }

        /// <summary>
        /// Sets value at the specified position. If there is value already present at that position, updates it with the
        /// result of the <paramref name="updater"/> function which is given the present value and the new value.
        /// </summary>
        public TValue SetOrElseUpdate(P1 p1, P2 p2, P3 p3, P4 p4, TValue value, Func<TValue, TValue, TValue> updater)
        {
            return SetOrElseUpdate(Product.Create(p1, p2, p3, p4), value, updater);
        }

        /// <summary>
        /// Transforms the current 4-dimensional cube into a 3-dimensional cube by excluding the dimension 1.
        /// All values whose position differ just in dimension 1 (their positions without dimension 1 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 1.
        /// </summary>
        public DataCube3<P2, P3, P4, TValue> RollUpDimension1(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube3<P2, P3, P4, TValue>, IProduct3<P2, P3, P4>>(
                position => position.ExceptValue1, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 1. The slices are 3-dimensional cubes without dimension 1 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P1, DataCube3<P2, P3, P4, TValue>> SliceDimension1()
        {
            var slices = new DataCube1<P1, DataCube3<P2, P3, P4, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue1, () => new DataCube3<P2, P3, P4, TValue>());
                slice.Set(position.ExceptValue1, value);
            });
            return slices;
        }

        /// <summary>
        /// Transforms the current 4-dimensional cube into a 3-dimensional cube by excluding the dimension 2.
        /// All values whose position differ just in dimension 2 (their positions without dimension 2 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 2.
        /// </summary>
        public DataCube3<P1, P3, P4, TValue> RollUpDimension2(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube3<P1, P3, P4, TValue>, IProduct3<P1, P3, P4>>(
                position => position.ExceptValue2, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 2. The slices are 3-dimensional cubes without dimension 2 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P2, DataCube3<P1, P3, P4, TValue>> SliceDimension2()
        {
            var slices = new DataCube1<P2, DataCube3<P1, P3, P4, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue2, () => new DataCube3<P1, P3, P4, TValue>());
                slice.Set(position.ExceptValue2, value);
            });
            return slices;
        }

        /// <summary>
        /// Transforms the current 4-dimensional cube into a 3-dimensional cube by excluding the dimension 3.
        /// All values whose position differ just in dimension 3 (their positions without dimension 3 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 3.
        /// </summary>
        public DataCube3<P1, P2, P4, TValue> RollUpDimension3(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube3<P1, P2, P4, TValue>, IProduct3<P1, P2, P4>>(
                position => position.ExceptValue3, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 3. The slices are 3-dimensional cubes without dimension 3 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P3, DataCube3<P1, P2, P4, TValue>> SliceDimension3()
        {
            var slices = new DataCube1<P3, DataCube3<P1, P2, P4, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue3, () => new DataCube3<P1, P2, P4, TValue>());
                slice.Set(position.ExceptValue3, value);
            });
            return slices;
        }

        /// <summary>
        /// Transforms the current 4-dimensional cube into a 3-dimensional cube by excluding the dimension 4.
        /// All values whose position differ just in dimension 4 (their positions without dimension 4 are the same) are 
        /// aggregated using the <paramref name="aggregator"/> function into one value. This value is stored into the new cube with the 
        /// position without dimension 4.
        /// </summary>
        public DataCube3<P1, P2, P3, TValue> RollUpDimension4(Func<TValue, TValue, TValue> aggregator)
        {
            return Transform<DataCube3<P1, P2, P3, TValue>, IProduct3<P1, P2, P3>>(
                position => position.ExceptValue4, 
                aggregator
            );
        }

        /// <summary>
        /// Slices the current cube in the dimension 4. The slices are 3-dimensional cubes without dimension 4 of
        /// the current cube. Returns a new 1-dimensional cube where the values are the slices and the positions are values in 
        /// the sliced dimension.
        /// </summary>
        public DataCube1<P4, DataCube3<P1, P2, P3, TValue>> SliceDimension4()
        {
            var slices = new DataCube1<P4, DataCube3<P1, P2, P3, TValue>>();
            ForEach((position, value) =>
            {
                var slice = slices.GetOrElseSet(position.ProductValue4, () => new DataCube3<P1, P2, P3, TValue>());
                slice.Set(position.ExceptValue4, value);
            });
            return slices;
        }
    }

}