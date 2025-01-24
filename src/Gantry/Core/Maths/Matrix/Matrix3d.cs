using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Matrix;

/// <summary>
///     A double precision floating point 3 by 3 matrix.
///     Primarily to support rotations
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public readonly struct Matrix3d
{
    /// <summary>
    ///     The first element of the first row.
    /// </summary>
    public readonly double M00;

    /// <summary>
    ///     The second element of the first row.
    /// </summary>
    public readonly double M01;

    /// <summary>
    ///     The third element of the first row.
    /// </summary>
    public readonly double M02;

    /// <summary>
    ///     The first element of the second row.
    /// </summary>
    public readonly double M10;

    /// <summary>
    ///     The second element of the second row.
    /// </summary>
    public readonly double M11;

    /// <summary>
    ///     The third element of the second row.
    /// </summary>
    public readonly double M12;

    /// <summary>
    ///     The first element of the third row.
    /// </summary>
    public readonly double M20;

    /// <summary>
    ///     The second element of the third row.
    /// </summary>
    public readonly double M21;

    /// <summary>
    ///     The third element of the third row.
    /// </summary>
    public readonly double M22;

    /// <summary>
    ///     Initialises a new instance of the <see cref="Matrix3d" /> class.
    /// </summary>
    /// <param name="m00">Element [0][0]</param>
    /// <param name="m01">Element [0][1]</param>
    /// <param name="m02">Element [0][2]</param>
    /// <param name="m10">Element [1][0]</param>
    /// <param name="m11">Element [1][1]</param>
    /// <param name="m12">Element [1][2]</param>
    /// <param name="m20">Element [2][0]</param>
    /// <param name="m21">Element [2][1]</param>
    /// <param name="m22">Element [2][2]</param>
    public Matrix3d(double m00, double m01, double m02,
        double m10, double m11, double m12,
        double m20, double m21, double m22)
    {
        M00 = m00;
        M01 = m01;
        M02 = m02;
        M10 = m10;
        M11 = m11;
        M12 = m12;
        M20 = m20;
        M21 = m21;
        M22 = m22;
    }

    /// <summary>
    ///     Constructs and initializes a Matrix3d from the specified 9 element array.
    ///     M00 = m[0], M01 = m[1], etc.
    /// </summary>
    /// <param name="m">The array of length 9 containing in order.</param>
    public Matrix3d(IReadOnlyList<double> m)
    {
        M00 = m[0];
        M01 = m[1];
        M02 = m[2];
        M10 = m[3];
        M11 = m[4];
        M12 = m[5];
        M20 = m[6];
        M21 = m[7];
        M22 = m[8];
    }

    /// <summary>
    ///     Constructs a new matrix with the same values as the Matrix3d parameter.
    ///     @param m1 The source matrix.
    /// </summary>
    /// <param name="m1">The m1.</param>
    public Matrix3d(Matrix3d m1)
    {
        M00 = m1.M00;
        M01 = m1.M01;
        M02 = m1.M02;
        M10 = m1.M10;
        M11 = m1.M11;
        M12 = m1.M12;
        M20 = m1.M20;
        M21 = m1.M21;
        M22 = m1.M22;
    }

    /// <summary>
    ///     Constructs and initialises a Matrix3d to all zeros.
    /// </summary>
    public static Matrix3d Zero =>
        new(
            0, 0, 0,
            0, 0, 0,
            0, 0, 0);


    /// <summary>
    ///     Returns a string that contains the values of this Matrix3d.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
        var nl = Environment.NewLine;

        return "[" + nl + " [" + M00 + "\t" + M01 + "\t" + M02 + "]" + nl +
               " [" + M10 + "\t" + M11 + "\t" + M12 + "]" + nl +
               " [" + M20 + "\t" + M21 + "\t" + M22 + "] ]";
    }

    /// <summary>
    ///     Sets this Matrix3d to identity.
    /// </summary>
    public Matrix3d SetIdentity()
    {
        return new(
            1, 0, 0,
            0, 1, 0,
            0, 0, 1);
    }

    /// <summary>
    ///     Sets the scale component of the current matrix by factoring out the
    ///     current scale (by doing an SVD) from the rotational component and
    ///     multiplying by the new scale.
    /// </summary>
    /// <param name="scale">The amount to scale by.</param>
    public Matrix3d SetScale(double scale)
    {
        var m = NormaliseSvd(this, out _);
        return new Matrix3d(m.M00 * scale, M01, M02, M10, M11 * scale, M12, M20, M21, M22 * scale);
    }

    /// <summary>
    ///     Retrieves the value at the specified row and column of this matrix.
    /// </summary>
    /// <param name="row">The row number to be retrieved (zero indexed).</param>
    /// <param name="column">The column number to be retrieved (zero indexed).</param>
    /// <returns>The value at the indexed element.</returns>
    /// <exception cref="IndexOutOfRangeException">Index outside the bounds of the matrix. Row: {row}, Column: {column}</exception>
    public double GetElement(int row, int column)
    {
        return (row, column) switch
        {
            var m00 when m00 == (0, 0) => M00,
            var m01 when m01 == (0, 1) => M01,
            var m02 when m02 == (0, 2) => M02,
            var m10 when m10 == (1, 0) => M10,
            var m11 when m11 == (1, 1) => M11,
            var m12 when m12 == (1, 2) => M12,
            var m20 when m20 == (2, 0) => M20,
            var m10 when m10 == (2, 1) => M21,
            var m22 when m22 == (2, 2) => M22,
            _ => throw new IndexOutOfRangeException(
                $"Index outside the bounds of the matrix. Row: {row}, Column: {column}")
        };
    }

    /// <summary>
    ///     Copies the matrix values in the specified row into the
    ///     array parameter.
    ///     @param row the matrix row
    ///     @param v The array into which the matrix row values will be copied
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="v">The v.</param>
    /// <exception cref="IndexOutOfRangeException">row must be 0 to 2 and is " + row</exception>
    public void GetRow(int row, double[] v)
    {
        switch (row)
        {
            case 0:
                v[0] = M00;
                v[1] = M01;
                v[2] = M02;
                break;
            case 1:
                v[0] = M10;
                v[1] = M11;
                v[2] = M12;
                break;
            case 2:
                v[0] = M20;
                v[1] = M21;
                v[2] = M22;
                break;
            default:
                throw new IndexOutOfRangeException("row must be 0 to 2 and is " + row);
        }
    }

    /// <summary>
    ///     Copies the matrix values in the specified row into the
    ///     vector parameter.
    ///     @param row the matrix row
    ///     @param v The vector into which the matrix row values will be copied
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="v">The v.</param>
    /// <exception cref="IndexOutOfRangeException">row must be 0 to 2 and is " + row</exception>
    public void GetRow(int row, Vec3d v)
    {
        switch (row)
        {
            case 0:
                v.X = M00;
                v.Y = M01;
                v.Z = M02;
                break;
            case 1:
                v.X = M10;
                v.Y = M11;
                v.Z = M12;
                break;
            case 2:
                v.X = M20;
                v.Y = M21;
                v.X = M22;
                break;
            default:
                throw new IndexOutOfRangeException("row must be 0 to 2 and is " + row);
        }
    }

    /// <summary>
    ///     Copies the matrix values in the specified column into the vector
    ///     parameter.
    ///     @param column the matrix column
    ///     @param v The vector into which the matrix row values will be copied
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="v">The v.</param>
    /// <exception cref="IndexOutOfRangeException">column must be 0 to 2 and is " + column</exception>
    public void GetColumn(int column, Vec3d v)
    {
        switch (column)
        {
            case 0:
                v.X = M00;
                v.Y = M10;
                v.Z = M20;
                break;
            case 1:
                v.X = M01;
                v.Y = M11;
                v.Z = M21;
                break;
            case 2:
                v.X = M02;
                v.Y = M12;
                v.Z = M22;
                break;
            default:
                throw new IndexOutOfRangeException("column must be 0 to 2 and is " + column);
        }
    }

    /// <summary>
    ///     Copies the matrix values in the specified column into the array
    ///     parameter.
    ///     @param column the matrix column
    ///     @param v The array into which the matrix row values will be copied
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="v">The v.</param>
    /// <exception cref="IndexOutOfRangeException">column must be 0 to 2 and is " + column</exception>
    public void GetColumn(int column, double[] v)
    {
        switch (column)
        {
            case 0:
                v[0] = M00;
                v[1] = M10;
                v[2] = M20;
                break;
            case 1:
                v[0] = M01;
                v[1] = M11;
                v[2] = M21;
                break;
            case 2:
                v[0] = M02;
                v[1] = M12;
                v[2] = M22;
                break;
            default:
                throw new IndexOutOfRangeException("column must be 0 to 2 and is " + column);
        }
    }

    /// <summary>
    ///     Performs an SVD normalization of this matrix to calculate and return the
    ///     uniform scale factor. This matrix is not modified.
    ///     @return the scale factor of this matrix
    /// </summary>
    /// <returns>System.Double.</returns>
    public double GetScale()
    {
        var _ = NormaliseSvd(null, out var scaleFactor);
        return scaleFactor;
    }


    /// <summary>
    ///     Adds a scalar to each component of this matrix.
    ///     @param scalar The scalar adder.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    public Matrix3d Add(double scalar)
    {
        return new(
            M00 + scalar, M01 + scalar, M02 + scalar,
            M10 + scalar, M11 + scalar, M12 + scalar,
            M20 + scalar, M21 + scalar, M22 + scalar);
    }

    /// <summary>
    ///     Sets the value of this matrix to the matrix sum of matrices.
    /// </summary>
    /// <param name="matrices">The list of matrices to sum together.</param>
    public static Matrix3d Sum(params Matrix3d[] matrices)
    {
        return new(
            matrices.Sum(p => p.M00),
            matrices.Sum(p => p.M01),
            matrices.Sum(p => p.M02),
            matrices.Sum(p => p.M10),
            matrices.Sum(p => p.M11),
            matrices.Sum(p => p.M12),
            matrices.Sum(p => p.M20),
            matrices.Sum(p => p.M21),
            matrices.Sum(p => p.M22)
        );
    }

    /// <summary>
    ///     Sets the value of this matrix to sum of itself and matrix m1.
    ///     @param m1 the other matrix
    /// </summary>
    /// <param name="m1">The m1.</param>
    public Matrix3d Add(Matrix3d m1)
    {
        return new(
            M00 + m1.M00, M01 + m1.M01, M02 + m1.M02,
            M10 + m1.M10, M11 + m1.M11, M12 + m1.M12,
            M20 + m1.M20, M21 + m1.M21, M22 + m1.M22);
    }


    /// <summary>
    ///     Sets the value of this matrix to its transpose.
    /// </summary>
    public Matrix3d Transpose()
    {
        return new(
            M00, M10, M20,
            M01, M11, M21,
            M02, M12, M22);
    }


    /// <summary>
    ///     Sets the value of this matrix to its inverse.
    /// </summary>
    public Matrix3d Invert()
    {
        var s = Determinant();
        if (s == 0.0)
            return this;
        s = 1 / s;
        var m = new Matrix3d(
            M11 * M22 - M12 * M21, M02 * M21 - M01 * M22, M01 * M12 - M02 * M11,
            M12 * M20 - M10 * M22, M00 * M22 - M02 * M20, M02 * M10 - M00 * M12,
            M10 * M21 - M11 * M20, M01 * M20 - M00 * M21, M00 * M11 - M01 * M10
        );

        return m.Mul(s);
    }

    /// <summary>
    ///     Computes the determinant of this matrix.
    ///     @return the determinant of the matrix
    /// </summary>
    /// <returns>System.Double.</returns>
    public double Determinant()
    {
        return M00 * (M11 * M22 - M21 * M12)
               - M01 * (M10 * M22 - M20 * M12)
               + M02 * (M10 * M21 - M20 * M11);
    }

    /// <summary>
    ///     Sets the value of this matrix to a scale matrix with the
    ///     passed scale amount.
    ///     @param scale the scale factor for the matrix
    /// </summary>
    /// <param name="scale">The scale.</param>
    public Matrix3d Scale(double scale)
    {
        return new(
            scale, 0.0, 0.0,
            0.0, scale, 0.0,
            0.0, 0.0, scale);
    }


    /// <summary>
    ///     Sets the value of this matrix to a rotation matrix about the x axis
    ///     by the passed angle.
    ///     @param angle the angle to rotate about the X axis in radians
    /// </summary>
    /// <param name="angle">The angle.</param>
    public Matrix3d RotX(double angle)
    {
        var c = Math.Cos(angle);
        var s = Math.Sin(angle);

        return new Matrix3d(
            1.0, 0.0, 0.0,
            0.0, c, -s,
            0.0, s, c);
    }

    /// <summary>
    ///     Sets the value of this matrix to a rotation matrix about the y axis
    ///     by the passed angle.
    ///     @param angle the angle to rotate about the Y axis in radians
    /// </summary>
    /// <param name="angle">The angle.</param>
    public static Matrix3d RotY(double angle)
    {
        var c = Math.Cos(angle);
        var s = Math.Sin(angle);

        return new Matrix3d(
            c, 0.0, s,
            0.0, 1.0, 0.0,
            -s, 0.0, c);
    }

    /// <summary>
    ///     Sets the value of this matrix to a rotation matrix about the z axis
    ///     by the passed angle.
    ///     @param angle the angle to rotate about the Z axis in radians
    /// </summary>
    /// <param name="angle">The angle.</param>
    public Matrix3d RotZ(double angle)
    {
        var c = Math.Cos(angle);
        var s = Math.Sin(angle);

        return new Matrix3d(
            c, -s, 0.0,
            s, c, 0.0,
            0.0, 0.0, 1.0);
    }

    /// <summary>
    ///     Multiplies each element of this matrix by a scalar.
    ///     @param scalar The scalar multiplier.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    public Matrix3d Mul(double scalar)
    {
        return new(
            M00 * scalar, M01 * scalar, M02 * scalar,
            M10 * scalar, M11 * scalar, M12 * scalar,
            M20 * scalar, M21 * scalar, M22 * scalar);
    }


    /// <summary>
    ///     Multiplies matrix m1 times the transpose of matrix m2, and places the
    ///     result into this.
    ///     @param m1 The matrix on the left hand side of the multiplication
    ///     @param m2 The matrix on the right hand side of the multiplication
    /// </summary>
    /// <param name="m1">The m1.</param>
    /// <param name="m2">The m2.</param>
    public Matrix3d MulTransposeRight(Matrix3d m1, Matrix3d m2)
    {
        return new(
            m1.M00 * m2.M00 + m1.M01 * m2.M01 + m1.M02 * m2.M02,
            m1.M00 * m2.M10 + m1.M01 * m2.M11 + m1.M02 * m2.M12,
            m1.M00 * m2.M20 + m1.M01 * m2.M21 + m1.M02 * m2.M22,
            m1.M10 * m2.M00 + m1.M11 * m2.M01 + m1.M12 * m2.M02,
            m1.M10 * m2.M10 + m1.M11 * m2.M11 + m1.M12 * m2.M12,
            m1.M10 * m2.M20 + m1.M11 * m2.M21 + m1.M12 * m2.M22,
            m1.M20 * m2.M00 + m1.M21 * m2.M01 + m1.M22 * m2.M02,
            m1.M20 * m2.M10 + m1.M21 * m2.M11 + m1.M22 * m2.M12,
            m1.M20 * m2.M20 + m1.M21 * m2.M21 + m1.M22 * m2.M22
        );
    }


    /// <summary>
    ///     Multiplies the transpose of matrix m1 times matrix m2, and places the
    ///     result into this.
    ///     @param m1 The matrix on the left hand side of the multiplication
    ///     @param m2 The matrix on the right hand side of the multiplication
    /// </summary>
    /// <param name="m1">The m1.</param>
    /// <param name="m2">The m2.</param>
    public Matrix3d MulTransposeLeft(Matrix3d m1, Matrix3d m2)
    {
        return new(
            m1.M00 * m2.M00 + m1.M10 * m2.M10 + m1.M20 * m2.M20,
            m1.M00 * m2.M01 + m1.M10 * m2.M11 + m1.M20 * m2.M21,
            m1.M00 * m2.M02 + m1.M10 * m2.M12 + m1.M20 * m2.M22,
            m1.M01 * m2.M00 + m1.M11 * m2.M10 + m1.M21 * m2.M20,
            m1.M01 * m2.M01 + m1.M11 * m2.M11 + m1.M21 * m2.M21,
            m1.M01 * m2.M02 + m1.M11 * m2.M12 + m1.M21 * m2.M22,
            m1.M02 * m2.M00 + m1.M12 * m2.M10 + m1.M22 * m2.M20,
            m1.M02 * m2.M01 + m1.M12 * m2.M11 + m1.M22 * m2.M21,
            m1.M02 * m2.M02 + m1.M12 * m2.M12 + m1.M22 * m2.M22
        );
    }

    /// <summary>
    ///     Performs singular value decomposition normalisation of this matrix.
    /// </summary>
    public Matrix3d Normalise()
    {
        return NormaliseSvd(this, out _);
    }

    /// <summary>
    ///     Perform cross product normalisation of this matrix.
    /// </summary>
    public Matrix3d NormaliseCp()
    {
        var s = Math.Pow(Math.Abs(Determinant()), -1.0 / 3.0);
        return Mul(s);
    }

    /// <summary>
    ///     Returns true if all of the data members of Matrix3d m1 are
    ///     equal to the corresponding data members in this Matrix3d.
    ///     @param m1 The matrix with which the comparison is made.
    ///     @return true or false
    /// </summary>
    /// <param name="m1">The m1.</param>
    /// <returns>boolean.</returns>
    public bool Equals(Matrix3d m1)
    {
        const int tolerance = 0;
        return Math.Abs(M00 - m1.M00) < tolerance
               && Math.Abs(M01 - m1.M01) < tolerance
               && Math.Abs(M02 - m1.M02) < tolerance
               && Math.Abs(M10 - m1.M10) < tolerance
               && Math.Abs(M11 - m1.M11) < tolerance
               && Math.Abs(M12 - m1.M12) < tolerance
               && Math.Abs(M20 - m1.M20) < tolerance
               && Math.Abs(M21 - m1.M21) < tolerance
               && Math.Abs(M22 - m1.M22) < tolerance;
    }

    /// <summary>
    ///     Returns true if the Object o1 is of type Matrix3d and all of the data
    ///     members of t1 are equal to the corresponding data members in this
    ///     Matrix3d.
    ///     @param o1 the object with which the comparison is made.
    /// </summary>
    public override bool Equals(object o1)
    {
        return o1 is Matrix3d matrix3d && Equals(matrix3d);
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var bits = BitConverter.DoubleToInt64Bits(M00);
            var hash = (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M01);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M02);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M10);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M11);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M12);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M20);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M21);
            hash ^= (int) (bits ^ (bits >> 32));
            bits = BitConverter.DoubleToInt64Bits(M22);
            hash ^= (int) (bits ^ (bits >> 32));
            return hash;
        }
    }

    /// <summary>
    ///     Returns true if the L-infinite distance between this matrix and matrix
    ///     m1 is less than or equal to the epsilon parameter, otherwise returns
    ///     false. The L-infinite distance is equal to MAX[i=0,1,2,3 ; j=0,1,2,3 ;
    ///     abs(this.m(i,j) - m1.m(i,j)]
    ///     @param m1 The matrix to be compared to this matrix
    ///     @param epsilon the threshold value
    /// </summary>
    public bool EpsilonEquals(Matrix3d m1, double epsilon)
    {
        return Math.Abs(M00 - m1.M00) <= epsilon
               && Math.Abs(M01 - m1.M01) <= epsilon
               && Math.Abs(M02 - m1.M02) <= epsilon
               && Math.Abs(M10 - m1.M10) <= epsilon
               && Math.Abs(M11 - m1.M11) <= epsilon
               && Math.Abs(M12 - m1.M12) <= epsilon
               && Math.Abs(M20 - m1.M20) <= epsilon
               && Math.Abs(M21 - m1.M21) <= epsilon
               && Math.Abs(M22 - m1.M22) <= epsilon;
    }

    /// <summary>
    ///     Negates the value of this matrix: this = -this.
    /// </summary>
    public Matrix3d Negate()
    {
        return new(
            -M00, -M01, -M02,
            -M10, -M11, -M12,
            -M20, -M21, -M22);
    }

    /// <summary>
    ///     Transform the vector vec using this Matrix3d and place the result back into vec.
    /// </summary>
    public void Transform(Vec3d t)
    {
        Transform(t, t);
    }

    /// <summary>
    ///     Transform the vector vec using this Matrix3d and place the result into vecOut.
    /// </summary>
    public void Transform(Vec3d t, Vec3d result)
    {
        result.Set(
            M00 * t.X + M01 * t.Y + M02 * t.Z,
            M10 * t.X + M11 * t.Y + M12 * t.Z,
            M20 * t.X + M21 * t.Y + M22 * t.Z
        );
    }

    /// <summary>
    ///     Performs SVD on this matrix and gets scale and rotation.
    ///     Rotation is placed into rot.
    ///     @param rot the rotation factor.
    ///     @return scale factor
    /// </summary>
    private Matrix3d NormaliseSvd(Matrix3d? rot, out double scaleFactor)
    {
        // this is a simple svd.
        // Not complete but fast and reasonable.

        // SVD scale factors(squared) are the 3 roots of
        //
        // | xI - M*MT | = 0.
        //
        // This will be expanded as follows
        // 
        // x^3 - A x^2 + B x - C = 0
        // 
        // where A, B, C can be denoted by 3 roots x0, x1, x2.
        // 
        // A = (x0+x1+x2), B = (x0x1+x1x2+x2x0), C = x0x1x2.
        // 
        // An average of x0,x1,x2 is needed here. C^(1/3) is a cross product normalisation factor.

        // So here, I use A/3. Note that x should be sqrt'ed for the actual factor.

        scaleFactor = Math.Sqrt(
            (
                M00 * M00 + M10 * M10 + M20 * M20 +
                M01 * M01 + M11 * M11 + M21 * M21 +
                M02 * M02 + M12 * M12 + M22 * M22
            ) / 3.0);

        if (rot == null) return new Matrix3d();

        var n0 = 1 / Math.Sqrt(M00 * M00 + M10 * M10 + M20 * M20);
        var n1 = 1 / Math.Sqrt(M01 * M01 + M11 * M11 + M21 * M21);
        var n2 = 1 / Math.Sqrt(M02 * M02 + M12 * M12 + M22 * M22);

        return new Matrix3d(
            M00 * n0, M01 * n1, M02 * n2,
            M10 * n0, M11 * n1, M12 * n2,
            M20 * n0, M21 * n1, M22 * n2);
    }

    /// <summary>
    ///     Converts a quaternion, to a <see cref="Matrix3d"/>
    /// </summary>
    public static Matrix3d FromQuaternion(double x, double y, double z, double w)
    {
        var n = x * x + y * y + z * z + w * w;
        var s = n > 0.0 ? 2.0 / n : 0.0;

        double xs = x * s, ys = y * s, zs = z * s;
        double wx = w * xs, wy = w * ys, wz = w * zs;
        double xx = x * xs, xy = x * ys, xz = x * zs;
        double yy = y * ys, yz = y * zs, zz = z * zs;

        return new Matrix3d(
            1.0 - (yy + zz), xy - wz, xz + wy,
            xy + wz, 1.0 - (xx + zz), yz - wx,
            xz - wy, yz + wx, 1.0 - (xx + yy));
    }

    /// <summary>
    ///     Converts axis angles to a <see cref="Matrix3d"/>
    /// </summary>
    public static Matrix3d FromAxisAngle(double x, double y, double z, double angle)
    {
        var n = Math.Sqrt(x * x + y * y + z * z);
        n = 1 / n;
        x *= n;
        y *= n;
        z *= n;

        var c = Math.Cos(angle);
        var s = Math.Sin(angle);

        var omc = 1.0 - c;

        var tmp01 = x * y * omc;
        var tmp02 = z * s;

        var tmp11 = x * z * omc;
        var tmp12 = y * s;

        var tmp21 = y * z * omc;
        var tmp22 = x * s;

        return new Matrix3d(
            c + x * x * omc, tmp01 - tmp02, tmp11 + tmp12,
            tmp01 + tmp02, c + y * y * omc, tmp21 - tmp22,
            tmp11 - tmp12, tmp21 + tmp22, c + z * z * omc);
    }
}