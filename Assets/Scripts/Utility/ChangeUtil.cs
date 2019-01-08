using UnityEngine;

// For changing vectors and colors
public static class ChangeUtil
{
	#region Reassign
	public static Vector2 X(this Vector2 v, float x)
	{
		v.x = x;
		return v;
	}

	public static Vector2 Y(this Vector2 v, float y)
	{
		v.y = y;
		return v;
	}

	public static Vector2 XY(this Vector2 v, float val)
	{
		v.x = val;
		v.y = val;
		return v;
	}

	public static Vector2 XY(this Vector2 v, float x, float y)
	{
		v.x = x;
		v.y = y;
		return v;
	}

	public static Vector3 X(this Vector3 v, float x)
	{
		v.x = x;
		return v;
	}

	public static Vector3 Y(this Vector3 v, float y)
	{
		v.y = y;
		return v;
	}

	public static Vector3 Z(this Vector3 v, float z)
	{
		v.z = z;
		return v;
	}

	public static Vector3 XY(this Vector3 v, float val)
	{
		v.x = val;
		v.y = val;
		return v;
	}

	public static Vector3 XY(this Vector3 v, float x, float y)
	{
		v.x = x;
		v.y = y;
		return v;
	}

	public static Vector3 XZ(this Vector3 v, float val)
	{
		v.x = val;
		v.z = val;
		return v;
	}

	public static Vector3 XZ(this Vector3 v, float x, float z)
	{
		v.x = x;
		v.z = z;
		return v;
	}

	public static Vector3 YZ(this Vector3 v, float val)
	{
		v.y = val;
		v.z = val;
		return v;
	}

	public static Vector3 YZ(this Vector3 v, float y, float z)
	{
		v.y = y;
		v.z = z;
		return v;
	}

	public static Vector3 XYZ(this Vector3 v, float val)
	{
		v.x = val;
		v.y = val;
		v.z = val;
		return v;
	}

	public static Vector3 XYZ(this Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
		return v;
	}

	public static Color R(this Color c, float r)
	{
		c.r = r;
		return c;
	}

	public static Color G(this Color c, float g)
	{
		c.g = g;
		return c;
	}

	public static Color B(this Color c, float b)
	{
		c.b = b;
		return c;
	}

	public static Color A(this Color c, float a)
	{
		c.a = a;
		return c;
	}
	#endregion

	#region Addition

	public static Vector2 XAdd(this Vector2 v, float x)
	{
		v.x += x;
		return v;
	}

	public static Vector2 YAdd(this Vector2 v, float y)
	{
		v.y += y;
		return v;
	}

	public static Vector2 XYAdd(this Vector2 v, float val)
	{
		v.x += val;
		v.y += val;
		return v;
	}

	public static Vector2 XYAdd(this Vector2 v, float x, float y)
	{
		v.x += x;
		v.y += y;
		return v;
	}

	public static Vector3 XAdd(this Vector3 v, float x)
	{
		v.x += x;
		return v;
	}

	public static Vector3 YAdd(this Vector3 v, float y)
	{
		v.y += y;
		return v;
	}

	public static Vector3 ZAdd(this Vector3 v, float z)
	{
		v.z += z;
		return v;
	}

	public static Vector3 XYAdd(this Vector3 v, float val)
	{
		v.x += val;
		v.y += val;
		return v;
	}

	public static Vector3 XYAdd(this Vector3 v, float x, float y)
	{
		v.x += x;
		v.y += y;
		return v;
	}

	public static Vector3 XZAdd(this Vector3 v, float val)
	{
		v.x += val;
		v.z += val;
		return v;
	}

	public static Vector3 XZAdd(this Vector3 v, float x, float z)
	{
		v.x += x;
		v.z += z;
		return v;
	}

	public static Vector3 YZAdd(this Vector3 v, float val)
	{
		v.y += val;
		v.z += val;
		return v;
	}

	public static Vector3 YZAdd(this Vector3 v, float y, float z)
	{
		v.y += y;
		v.z += z;
		return v;
	}

	public static Vector3 XYZAdd(this Vector3 v, float val)
	{
		v.x += val;
		v.y += val;
		v.z += val;
		return v;
	}

	public static Vector3 XYZAdd(this Vector3 v, float x, float y, float z)
	{
		v.x += x;
		v.y += y;
		v.z += z;
		return v;
	}

	public static Color RAdd(this Color c, float r)
	{
		c.r += r;
		return c;
	}

	public static Color GAdd(this Color c, float g)
	{
		c.g += g;
		return c;
	}

	public static Color BAdd(this Color c, float b)
	{
		c.b += b;
		return c;
	}

	public static Color AAdd(this Color c, float a)
	{
		c.a += a;
		return c;
	}
	#endregion

	#region Subtraction

	public static Vector2 XSub(this Vector2 v, float x)
	{
		v.x -= x;
		return v;
	}

	public static Vector2 YSub(this Vector2 v, float y)
	{
		v.y -= y;
		return v;
	}

	public static Vector2 XYSub(this Vector2 v, float val)
	{
		v.x -= val;
		v.y -= val;
		return v;
	}

	public static Vector2 XYSub(this Vector2 v, float x, float y)
	{
		v.x -= x;
		v.y -= y;
		return v;
	}

	public static Vector3 XSub(this Vector3 v, float x)
	{
		v.x -= x;
		return v;
	}

	public static Vector3 YSub(this Vector3 v, float y)
	{
		v.y -= y;
		return v;
	}

	public static Vector3 ZSub(this Vector3 v, float z)
	{
		v.z -= z;
		return v;
	}

	public static Vector3 XYSub(this Vector3 v, float val)
	{
		v.x -= val;
		v.y -= val;
		return v;
	}

	public static Vector3 XYSub(this Vector3 v, float x, float y)
	{
		v.x -= x;
		v.y -= y;
		return v;
	}

	public static Vector3 XZSub(this Vector3 v, float val)
	{
		v.x -= val;
		v.z -= val;
		return v;
	}

	public static Vector3 XZSub(this Vector3 v, float x, float z)
	{
		v.x -= x;
		v.z -= z;
		return v;
	}

	public static Vector3 YZSub(this Vector3 v, float val)
	{
		v.y -= val;
		v.z -= val;
		return v;
	}

	public static Vector3 YZSub(this Vector3 v, float y, float z)
	{
		v.y -= y;
		v.z -= z;
		return v;
	}

	public static Vector3 XYZSub(this Vector3 v, float val)
	{
		v.x -= val;
		v.y -= val;
		v.z -= val;
		return v;
	}

	public static Vector3 XYZSub(this Vector3 v, float x, float y, float z)
	{
		v.x -= x;
		v.y -= y;
		v.z -= z;
		return v;
	}

	public static Color RSub(this Color c, float r)
	{
		c.r -= r;
		return c;
	}

	public static Color GSub(this Color c, float g)
	{
		c.g -= g;
		return c;
	}

	public static Color BSub(this Color c, float b)
	{
		c.b -= b;
		return c;
	}

	public static Color ASub(this Color c, float a)
	{
		c.a -= a;
		return c;
	}
	#endregion

	#region Multiplication

	public static Vector2 XMul(this Vector2 v, float x)
	{
		v.x *= x;
		return v;
	}

	public static Vector2 YMul(this Vector2 v, float y)
	{
		v.y *= y;
		return v;
	}

	public static Vector2 XYMul(this Vector2 v, float val)
	{
		v.x *= val;
		v.y *= val;
		return v;
	}

	public static Vector2 XYMul(this Vector2 v, float x, float y)
	{
		v.x *= x;
		v.y *= y;
		return v;
	}

	public static Vector3 XMul(this Vector3 v, float x)
	{
		v.x *= x;
		return v;
	}

	public static Vector3 YMul(this Vector3 v, float y)
	{
		v.y *= y;
		return v;
	}

	public static Vector3 ZMul(this Vector3 v, float z)
	{
		v.z *= z;
		return v;
	}

	public static Vector3 XYMul(this Vector3 v, float val)
	{
		v.x *= val;
		v.y *= val;
		return v;
	}

	public static Vector3 XYMul(this Vector3 v, float x, float y)
	{
		v.x *= x;
		v.y *= y;
		return v;
	}

	public static Vector3 XZMul(this Vector3 v, float val)
	{
		v.x *= val;
		v.z *= val;
		return v;
	}

	public static Vector3 XZMul(this Vector3 v, float x, float z)
	{
		v.x *= x;
		v.z *= z;
		return v;
	}

	public static Vector3 YZMul(this Vector3 v, float val)
	{
		v.y *= val;
		v.z *= val;
		return v;
	}

	public static Vector3 YZMul(this Vector3 v, float y, float z)
	{
		v.y *= y;
		v.z *= z;
		return v;
	}

	public static Vector3 XYZMul(this Vector3 v, float val)
	{
		v.x *= val;
		v.y *= val;
		v.z *= val;
		return v;
	}

	public static Vector3 XYZMul(this Vector3 v, float x, float y, float z)
	{
		v.x *= x;
		v.y *= y;
		v.z *= z;
		return v;
	}

	public static Color RMul(this Color c, float r)
	{
		c.r *= r;
		return c;
	}

	public static Color GMul(this Color c, float g)
	{
		c.g *= g;
		return c;
	}

	public static Color BMul(this Color c, float b)
	{
		c.b *= b;
		return c;
	}

	public static Color AMul(this Color c, float a)
	{
		c.a *= a;
		return c;
	}
	#endregion

	#region Division

	public static Vector2 XDiv(this Vector2 v, float x)
	{
		v.x /= x;
		return v;
	}

	public static Vector2 YDiv(this Vector2 v, float y)
	{
		v.y /= y;
		return v;
	}

	public static Vector2 XYDiv(this Vector2 v, float val)
	{
		v.x /= val;
		v.y /= val;
		return v;
	}

	public static Vector2 XYDiv(this Vector2 v, float x, float y)
	{
		v.x /= x;
		v.y /= y;
		return v;
	}

	public static Vector3 XDiv(this Vector3 v, float x)
	{
		v.x /= x;
		return v;
	}

	public static Vector3 YDiv(this Vector3 v, float y)
	{
		v.y /= y;
		return v;
	}

	public static Vector3 ZDiv(this Vector3 v, float z)
	{
		v.z /= z;
		return v;
	}

	public static Vector3 XYDiv(this Vector3 v, float val)
	{
		v.x /= val;
		v.y /= val;
		return v;
	}

	public static Vector3 XYDiv(this Vector3 v, float x, float y)
	{
		v.x /= x;
		v.y /= y;
		return v;
	}

	public static Vector3 XZDiv(this Vector3 v, float val)
	{
		v.x /= val;
		v.z /= val;
		return v;
	}

	public static Vector3 XZDiv(this Vector3 v, float x, float z)
	{
		v.x /= x;
		v.z /= z;
		return v;
	}

	public static Vector3 YZDiv(this Vector3 v, float val)
	{
		v.y /= val;
		v.z /= val;
		return v;
	}

	public static Vector3 YZDiv(this Vector3 v, float y, float z)
	{
		v.y /= y;
		v.z /= z;
		return v;
	}

	public static Vector3 XYZDiv(this Vector3 v, float val)
	{
		v.x /= val;
		v.y /= val;
		v.z /= val;
		return v;
	}

	public static Vector3 XYZDiv(this Vector3 v, float x, float y, float z)
	{
		v.x /= x;
		v.y /= y;
		v.z /= z;
		return v;
	}

	public static Color RDiv(this Color c, float r)
	{
		c.r /= r;
		return c;
	}

	public static Color GDiv(this Color c, float g)
	{
		c.g /= g;
		return c;
	}

	public static Color BDiv(this Color c, float b)
	{
		c.b /= b;
		return c;
	}

	public static Color ADiv(this Color c, float a)
	{
		c.a /= a;
		return c;
	}
	#endregion
}