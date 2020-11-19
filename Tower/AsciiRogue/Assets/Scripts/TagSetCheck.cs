using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSetCheck 
{
    string myTag;

    private System.Func<HashSet<string>, bool> myCheck;


    // this allows for shorter writing so you dont have to call check
    public bool this[HashSet<string> toCheck] => Check(toCheck);


    public TagSetCheck(string Tag)
    {
        myTag = Tag;
        myCheck = (t) => t.Contains(myTag);
    }

    private TagSetCheck()
    {

    }

    public bool Check(HashSet<string> tag)
    {
        return myCheck(tag);
    }

    public static TagSetCheck operator |(TagSetCheck a, TagSetCheck b)
    {
        return new TagSetCheck() { myCheck = (t) => a.Check(t) || b.Check(t) };
    }

    public static TagSetCheck operator &(TagSetCheck a, TagSetCheck b)
    {
        return new TagSetCheck() { myCheck = (t) => a.Check(t) && b.Check(t) };
    }
    public static TagSetCheck operator !(TagSetCheck a)
    {
        return new TagSetCheck() { myCheck = (t) => !a.Check(t) };
    }

    public static implicit operator TagSetCheck(string tag)
    {
        return new TagSetCheck(tag);
    }


}
