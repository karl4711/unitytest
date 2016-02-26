using UnityEngine;
using System.Collections;
using LuaInterface;

public class TestScript1 : MonoBehaviour {

    private string script = @"
            function luaFunc(message)
                print(message)
                return 42
            end
        ";


    void Awake () {
        LuaState l = new LuaState();

        l.DoString(script);

        LuaFunction f = l.GetFunction("luaFunc");

        object[] r = f.Call("I called a lua function!");

        print(r[0]);
        Debug.Log("Test script 1 AWAKE.");	
	}

	public static void print(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("MainTest");
		print ("test print");
	}
}
