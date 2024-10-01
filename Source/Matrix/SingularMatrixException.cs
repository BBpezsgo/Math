using System;

namespace Maths
{
    /// <summary>
    /// This matrix has no inverse
    /// </summary>
    public class SingularMatrixException : Exception
    {
        public IMatrix Matrix { get; }

        public SingularMatrixException(IMatrix matrix) : base("This matrix has no inverse")
        {
            Matrix = matrix;
        }
    }
}
