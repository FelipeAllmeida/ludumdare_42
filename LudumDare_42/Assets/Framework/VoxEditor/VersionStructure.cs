using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public enum VersionType
{
    VERSION,
    MAJOR_RELEASE,
    MINOR_RELEASE,
    DEVELOPMENT_RELEASE,
    BUNDLE_VERSION
}

public abstract class VersionStructure
{
    public VersionStructure(int p_number)
    {
        number = p_number;
    }

    public int number;

    public VersionType versionType;

    public virtual void Increase(VersionType p_versionType) { if (p_versionType == GetVersionType()) number++; }

    public virtual void Reset() { number = 0; }

    public abstract VersionType GetVersionType();
       
}

public class Version : VersionStructure
{
    private Version(int p_number) : base(p_number) { }
    public MajorRelease majorRelease;
    public BundleVersion bundleVersion;

    public override void Increase(VersionType p_versionType)
    {
        if (majorRelease != null )
        {
            if (p_versionType != GetVersionType())
            {
                majorRelease.Increase(p_versionType);
            }
            else
            {
                majorRelease.Reset();
            }
        }

        if (bundleVersion != null)
        {
            bundleVersion.Increase(VersionType.BUNDLE_VERSION);
        }
        
        base.Increase(p_versionType);
    }

    public override void Reset()
    {
        if (majorRelease != null)
        {
            majorRelease.Reset();
        }

        if (bundleVersion != null)
        {
            bundleVersion.Reset();
        }

        base.Reset();
    }

    public static Version CreateVersion(string p_version)
    {
        List<string> __listString = p_version.Split('.', '_').ToList();
        Version __version = new Version(int.Parse(__listString[0]));
        __listString.RemoveAt(0);

        string __bundleVersion = string.Empty;
        if (p_version.Contains("_"))
        {
            __bundleVersion = __listString[__listString.Count - 1];
            __listString.RemoveAt(__listString.Count - 1);
        }

        if (p_version.Contains("."))
        {
            __version.majorRelease = MajorRelease.CreateMajorRelease(__listString);
        }

        if (p_version.Contains("_"))
        {
            __version.bundleVersion = BundleVersion.CreateBundleVersion(__bundleVersion);
        }

        return __version;
    }

    public override VersionType GetVersionType()
    {
        return VersionType.VERSION;
    }

    public override string ToString()
    {
        string __version = number.ToString();

        if (majorRelease != null)
        {
            __version += majorRelease.ToString();
        }

        if (bundleVersion != null)
        {
            __version += bundleVersion.ToString();
        }

        return __version;
    }
}

public class MajorRelease : VersionStructure
{
    public MajorRelease(int p_number) : base(p_number) { }
    public MinorRelease minorRelease;

    public override void Increase(VersionType p_versionType)
    {
        if (minorRelease != null)
        {
            if (p_versionType != GetVersionType())
            {
                minorRelease.Increase(p_versionType);
            }
            else
            {
                minorRelease.Reset();
            }
        }
        base.Increase(p_versionType);
    }

    public override void Reset()
    {
        if (minorRelease != null)
        {
            minorRelease.Reset();
        }
        base.Reset();
    }

    public static MajorRelease CreateMajorRelease(List<string> p_listString)
    {
        MajorRelease __majorRelease = new MajorRelease(int.Parse(p_listString[0]));

        if (p_listString.Count > 1)
        {
            p_listString.RemoveAt(0);
            __majorRelease.minorRelease = MinorRelease.CreateMinorRelease(p_listString);
        }
        return __majorRelease;
    }

    public override VersionType GetVersionType()
    {
        return VersionType.MAJOR_RELEASE;
    }

    public override string ToString()
    {
        string __version = string.Format(".{0}", number);

        if (minorRelease != null)
        {
            __version += minorRelease.ToString();
        }

        return __version;
    }
}

public class MinorRelease : VersionStructure
{
    public MinorRelease(int p_number) : base(p_number) { }
    public DevelopmentRelease developmentRelease;

    public override void Increase(VersionType p_versionType)
    {
        if (developmentRelease != null)
        {
            if (p_versionType != GetVersionType())
            {
                developmentRelease.Increase(p_versionType);
            }
            else
            {
                developmentRelease.Reset();
            }
        }
        base.Increase(p_versionType);
    }

    public override void Reset()
    {
        if (developmentRelease != null)
        {
            developmentRelease.Reset();
        }
        base.Reset();
    }

    public static MinorRelease CreateMinorRelease(List<string> p_listString)
    {

        MinorRelease __minorRelease = new MinorRelease(int.Parse(p_listString[0]));

        if (p_listString.Count > 1)
        {
            p_listString.RemoveAt(0);
            __minorRelease.developmentRelease = DevelopmentRelease.CreateDevelopmentRelease(p_listString);
        }
        return __minorRelease;
    }

    public override VersionType GetVersionType()
    {
        return VersionType.MINOR_RELEASE;
    }

    public override string ToString()
    {
        string __version = string.Format(".{0}", number);

        if (developmentRelease != null)
        {
            __version += developmentRelease.ToString();
        }

        return __version;
    }
}

public class DevelopmentRelease : VersionStructure
{
    public DevelopmentRelease(int p_number) : base(p_number) { }

    public static DevelopmentRelease CreateDevelopmentRelease(List<string> p_listString)
    {
        return new DevelopmentRelease(int.Parse(p_listString[0]));
    }

    public override VersionType GetVersionType()
    {
        return VersionType.DEVELOPMENT_RELEASE;
    }

    public override string ToString()
    {
        return string.Format(".{0}", number);
    }
}

public class BundleVersion : VersionStructure
{
    public BundleVersion(int p_number) : base(p_number) { }
    public static BundleVersion CreateBundleVersion(string p_bundleVersion)
    {
        return new BundleVersion(int.Parse(p_bundleVersion));
    }

    public override VersionType GetVersionType()
    {
        return VersionType.BUNDLE_VERSION;
    }

    public override string ToString()
    {
        return string.Format("_{0}", number);
    }
}

