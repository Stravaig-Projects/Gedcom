.. _refPopulatingGedcomDatabase:

Populating the GedcomDatabase
=============================

There are a number of ways to get data into the ``GedcomDatabase`` object.

Populating from a file
----------------------

::

    public static void Main(string[] args)
    {
      var filePath = args[0];
      GedcomDatabase db = new GedcomDatabase();
      db.PopulateFromFile(filePath);
      // Do what ever you need to do with the database
    }

or, if you want the asynchronous version:

::

    public static async Task Main(string[] args)
    {
      var filePath = args[0];
      GedcomDatabase db = new GedcomDatabase();
      await db.PopulateFromFileAsync(filePath);
      // Do what ever you need to do with the database
    }


This will open the file, read the contents into the database and close the file again. It won't hold on to the file once the operation is completed.

Populating from a Stream
------------------------

You can populate the database from a ``Stream`` such as the example below which uses the ``Stream`` obtained from Blazor WebAssembly's ``IFileListEntry`` when uploading a file.

::

    async Task HandleFileSelectedAsync(IFileListEntry[] files)
    {
      var file = files.FirstOrDefault();
      if (file != null && file.Name.EndsWith(".ged", true, CultureInfo.InvariantCulture))
      {
        var db = new GedcomDatabase();
        await db.PopulateAsync(file.Data);
        // Do what ever you need to do with the database
      }
    }

There is also a synchronous version ``await db.Populate(file.Data)``

Populating from a TextReader
----------------------------

You can populate the database from any derivative of a ``TextReader`` such as a ``StringReader`` (which is backed by a ``StringBuilder``)

For example:

::

    StringReader reader = GetGedcomData();
    GedcomDatabase db = new GedcomDatabase();
    db.Populate(reader);

There is also an async version available ``await db.PopulateAsync(reader)``

