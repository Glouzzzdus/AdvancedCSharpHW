﻿using AdvancedCSharp;


class Program
{
    static void Main(string[] args)
    {
        var rootPath = @"C:\OSTState";

        var fileSystemVisitor = new FileSystemVisitor(rootPath, fsi => fsi is FileInfo fi && fi.Extension == ".ini");

        // subscribing
        fileSystemVisitor.Start += (sender, eventArgs) => Console.WriteLine("Traversal started.");
        fileSystemVisitor.Finish += (sender, eventArgs) => Console.WriteLine("Traversal finished.");
        fileSystemVisitor.FileFound += (fsi, eventArgs) => Console.WriteLine($"File found: {fsi.FullName}");
        fileSystemVisitor.DirectoryFound += (fsi, eventArgs) => Console.WriteLine($"Directory found: {fsi.FullName}");
        fileSystemVisitor.FilteredFileFound += (fsi, eventArgs) =>
        {
            Console.WriteLine($"Filtered file found: {fsi.FullName}");
            // terminate condition
            if (fsi.Name == "config.ini") 
            {
                eventArgs.Terminate = true;
            }
        };
        fileSystemVisitor.FilteredDirectoryFound += (fsi, eventArgs) => Console.WriteLine($"Filtered directory found: {fsi.FullName}");

        // traversing start
        foreach (var fsi in fileSystemVisitor.Traverse())
        {
            Console.WriteLine(fsi.FullName);
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

