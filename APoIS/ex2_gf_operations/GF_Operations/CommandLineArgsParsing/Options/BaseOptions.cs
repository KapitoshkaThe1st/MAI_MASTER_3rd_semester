using System;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    public class BaseOptions
    {
        protected void ThrowNotSupported()
        {
            throw new NotSupportedException(@"This prooperty is public for argument parsing and can't be private
                (thanks to CommandLine library for not providing argument validation out of the box). Use 'Number' property instead.");
        }
    }
}
