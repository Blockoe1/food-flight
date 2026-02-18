/*****************************************************************************
// File Name : MathHelpers.cs
// Author : Brandon Koederitz
// Creation Date : September 7, 2025
// Last Modified : September 7, 2025
//
// Brief Description : Set of static helper functions containing commonly used math equasions
// that have common utility across multiple projects.
*****************************************************************************/
using UnityEngine;

public static class MathHelpers
{
    #region Clamp
    /// <summary>
    /// Clamps all values of a vector3int between two given ints.
    /// </summary>
    /// <param name="input">The vector to clamp.</param>
    /// <param name="min">The min possible value.</param>
    /// <param name="max">The max possible value.</param>
    /// <returns>The Vector3Int with all of it's components clamped between the min and max values.</returns>
    public static Vector3Int V3IntClamp(Vector3Int input, int min, int max)
    {
        input.x = Mathf.Clamp(input.x, min, max);
        input.y = Mathf.Clamp(input.y, min, max);
        input.z = Mathf.Clamp(input.z, min, max);
        return input;
    }

    /// <summary>
    /// Clamps all values of a vector3 between two given ints.
    /// </summary>
    /// <param name="input">The vector to clamp.</param>
    /// <param name="min">The min possible value.</param>
    /// <param name="max">The max possible value.</param>
    /// <returns>The Vector3Int with all of it's components clamped between the min and max values.</returns>
    public static Vector3 V3Clamp(Vector3 input, int min, int max)
    {
        input.x = Mathf.Clamp(input.x, min, max);
        input.y = Mathf.Clamp(input.y, min, max);
        input.z = Mathf.Clamp(input.z, min, max);
        return input;
    }
    #endregion


    /// <summary>
    /// Gets the absolute value of a vector's components.
    /// </summary>
    /// <param name="input">The input vector.</param>
    /// <returns>The passed in vector with all of its components as positve values.</returns>
    public static Vector3Int V3IntAbs(Vector3Int input)
    {
        input.x = Mathf.Abs(input.x);
        input.y = Mathf.Abs(input.y);
        input.z = Mathf.Abs(input.z);
        return input;
    }

    /// <summary>
    /// Returns the canonical modulus of a number to the mod of another number
    /// </summary>
    /// <remarks>
    /// Differs from % in that % gives the remainder, which can be negative.  In this case, negative numbers loop 
    /// around.
    /// </remarks>
    /// <param name="x">The first neumber</param>
    /// <param name="m">The number to take the mod of.</param>
    /// <returns>The canonical modulus number of X mod M.</returns>
    public static int Mod(int x, int m)
    {
        return ((x % m) + m) % m;
    }

    #region GetSign
    /// <summary>
    /// Gets the sign of a given number
    /// </summary>
    /// <param name="x">The number to get the sign of.</param>
    /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
    public static int GetSign(int x)
    {
        if (x == 0) { return 0; }
        return Mathf.Abs(x) / x;
    }

    /// <summary>
    /// Gets the sign of a given number
    /// </summary>
    /// <param name="x">The number to get the sign of.</param>
    /// <returns>-1, 0, or 1, depending ont the sign of the number.</returns>
    public static int GetSign(float x)
    {
        if (x == 0) { return 0; }
        return (int)(Mathf.Abs(x) / x);
    }
    #endregion

    #region Angle to Vector
    /// <summary>
    /// Converts an angle in degrees to a unit vector pointing in that direction.
    /// </summary>
    /// <param name="angle">The angle to convert to a vector.</param>
    /// <returns>A Vector2 corresponding to that angle.</returns>
    public static Vector2 DegAngleToUnitVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
    }
    #endregion

    #region Vector to Angle
    /// <summary>
    /// Converts a vector in unity world space to an angle in degrees.
    /// </summary>
    /// <param name="vector">The vector to convert to an angle.</param>
    /// <returns>The angle that corresponds to that vector.</returns>
    public static float VectorToDegAngleUnity(Vector2 vector)
    {
        return Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Converts a vector to an angle in degrees.
    /// </summary>
    /// <param name="vector">The vector to convert to an angle.</param>
    /// <returns>The angle that corresponds to that vector.</returns>
    public static float VectorToDegAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
    #endregion

    /// <summary>
    /// Finds the Manhatten distance (or the total number of spaces) between two vector2ints.
    /// </summary>
    /// <param name="tile1">The first tile.</param>
    /// <param name="tile2">The second tile.</param>
    /// <returns>The total number of spaces between the two tiles.</returns>
    public static int FindManhattenDistance(Vector2Int tile1, Vector2Int tile2)
    {
        return Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
    }

    /// <summary>
    /// Restricts an angle to values between 0 and 360 degrees.
    /// </summary>
    /// <param name="angle">The angle to convert</param>
    /// <returns>The angle that corresponds to a given angle within 360 degrees.</returns>
    public static float RestrictAngle(float angle)
    {
        angle = angle % 360;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    #region ApproximatelyWithin

    /// <summary>
    /// Checks to see if two numbers are within a certain range of each other
    /// </summary>
    /// <param name="number1"> A number to compare. </param>
    /// <param name="number2"> A second number to compare. </param>
    /// <param name="range"> The numerical range that the two must be within. (Inclusive) </param>
    /// <returns> Whether the two numbers are within range of each other. </returns>
    public static bool ApproximatelyWithin(float number1, float number2, float range = 1E-6f)
    {
        float delta = number1 - number2;
        return Mathf.Abs(delta) <= range;
    }

    /// <summary>
    /// Checks to see if two vectors are within a certain range of each other
    /// </summary>
    /// <param name="vector1"> A vector to compare. </param>
    /// <param name="vector2"> A second vector to compare. </param>
    /// <param name="range"> The numerical range that the two must be within. (Inclusive) </param>
    /// <returns> Whether the two vectors are within range of each other. </returns>
    public static bool ApproximatelyWithin(Vector3 vector1, Vector3 vector2, float range = 1E-6f)
    {
        float xDelta = vector2.x - vector1.x;
        float yDelta = vector2.y - vector1.y;
        float zDelta = vector2.z - vector1.z;
        float distance = Mathf.Sqrt(Mathf.Pow(xDelta, 2) + Mathf.Pow(yDelta, 2) + Mathf.Pow(zDelta, 2));
        return Mathf.Abs(distance) <= range;
    }
    #endregion

    /// <summary>
    /// Converts a vector in cartesian coordinates into polar coordinates.
    /// </summary>
    /// <param name="cartesianCoordinates">The vecctor in cartesian coordiantes to convert. </param>
    /// <returns>
    /// The vector in polar coordinates, with magnitude as the x component and angle in degrees as the y component.
    /// </returns>
    public static Vector2 CartesianToPolarCoordinates(Vector2 cartesianCoordinates)
    {
        float angle = VectorToDegAngle(cartesianCoordinates);
        return new Vector2(cartesianCoordinates.magnitude, angle);
    }

        /// <summary>
    /// Rounds a vector2 to a Vector2Int
    /// </summary>
    /// <remarks>
    /// Does not accurately keep the vector's direction.  Do not use for direction vectors.
    /// </remarks>
    /// <param name="vector">The vector to round</param>
    /// <returns>The vector with it's components rounded to an int.</returns>
    public static Vector2Int RoundVectorToInt(Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }
}