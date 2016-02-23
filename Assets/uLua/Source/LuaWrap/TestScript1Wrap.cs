using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class TestScript1Wrap
{
	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("print", print),
			new LuaMethod("New", _CreateTestScript1),
			new LuaMethod("GetClassType", GetClassType),
			new LuaMethod("__eq", Lua_Eq),
		};

		LuaField[] fields = new LuaField[]
		{
		};

		LuaScriptMgr.RegisterLib(L, "TestScript1", typeof(TestScript1), regs, fields, typeof(MonoBehaviour));
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateTestScript1(IntPtr L)
	{
		LuaDLL.luaL_error(L, "TestScript1 class does not have a constructor function");
		return 0;
	}

	static Type classType = typeof(TestScript1);

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, classType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int print(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		TestScript1.print();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Object arg0 = LuaScriptMgr.GetLuaObject(L, 1) as Object;
		Object arg1 = LuaScriptMgr.GetLuaObject(L, 2) as Object;
		bool o = arg0 == arg1;
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

