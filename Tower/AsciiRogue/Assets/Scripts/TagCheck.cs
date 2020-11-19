using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagCheck
{

    string myTag;

    private System.Func<string, bool> myCheck;

    public bool this[string toCheck] => Check(toCheck);


    public TagCheck(string Tag)
    {
        myTag = Tag;
        myCheck = (t) => myTag == t;
    }

    private TagCheck()
    {

    }
    
    public bool Check(string tag)
    {
        return myCheck(tag);
    }

    public static TagCheck operator |(TagCheck a, TagCheck b)
    {
        return new TagCheck() { myCheck = (t) => a.Check(t) || b.Check(t) };
    }

    public static TagCheck operator &(TagCheck a, TagCheck b)
    {
        return new TagCheck() { myCheck = (t) => a.Check(t) && b.Check(t) };
    }
    public static TagCheck operator !(TagCheck a)
    {
        return new TagCheck() { myCheck = (t) => !a.Check(t)};
    }

    public static explicit operator TagCheck(string tag)
    {
        return new TagCheck(tag);
    }



}
