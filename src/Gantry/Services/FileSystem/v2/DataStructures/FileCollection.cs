using System.Collections;
using System.Collections.Generic;
using Gantry.Services.FileSystem.v2.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Services.FileSystem.v2.DataStructures
{
    /// <summary>
    ///     Default implementation of <see cref="IFileCollection" />.
    /// </summary>
    /// <seealso cref="IFileCollection" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class FileCollection : IFileCollection
    {
        private readonly List<FileDescriptor> _descriptors = new();

        /// <inheritdoc />
        public int Count => _descriptors.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public FileDescriptor this[int index]
        {
            get => _descriptors[index];
            set => _descriptors[index] = value;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _descriptors.Clear();
        }

        /// <inheritdoc />
        public bool Contains(FileDescriptor item)
        {
            return _descriptors.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(FileDescriptor[] array, int arrayIndex)
        {
            _descriptors.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(FileDescriptor item)
        {
            return _descriptors.Remove(item);
        }

        /// <inheritdoc />
        public IEnumerator<FileDescriptor> GetEnumerator()
        {
            return _descriptors.GetEnumerator();
        }

        void ICollection<FileDescriptor>.Add(FileDescriptor item)
        {
            _descriptors.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf(FileDescriptor item)
        {
            return _descriptors.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, FileDescriptor item)
        {
            _descriptors.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            _descriptors.RemoveAt(index);
        }
    }
}