using System;

namespace Maths
{
    /// <summary>
    /// This matrix has no inverse
    /// </summary>
#pragma warning disable RCS1194 // Implement exception constructors
    public class SingularMatrixException : Exception
#pragma warning restore RCS1194
    {
        public IMatrix Matrix { get; }

        public SingularMatrixException(IMatrix matrix) : base("This matrix has no inverse")
        {
            Matrix = matrix;
        }
    }
}
