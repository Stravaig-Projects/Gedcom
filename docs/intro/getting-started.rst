Getting Started
===============

Create a project and :ref:`install <refInstalling>` the ``Stravaig.Gedcom`` package into it. 

Creating the GedcomDatabase
---------------------------

The ``GedcomDatabase`` is the 

To load a GEDCOM file from a file:

::

    string filePath = "C:\my-family-tree\tree.ged";
    var database = new GedcomDatabase();
    database.PopulateFromFile(filePath);

Note: ``PopulateFromFileAsync()`` is also available.

There are also other ways to initially populate the ``GedcomDatabase`` (See :ref:`populating the GedcomDatabase <refPopulatingGedcomDatabase>` for more information)

