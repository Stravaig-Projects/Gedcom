using System;
using System.IO;
using System.Management.Automation;
using Stravaig.Gedcom.Model;
using Stravaig.Gedcom.Model.Extensions;
using Stravaig.Gedcom.Settings;

namespace Stravaig.Gedcom.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "GedcomDatabase")]
    [OutputType(typeof(PSGedcomDatabase))]
    public class GetGedcomDatabase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Path { get; set; }

        [ValidateRange(1,200)]
        [Parameter(Mandatory = false)]
        public int AssumedDeathAge { get; set; } = 100;

        [Parameter(Mandatory = false)]
        public DateOrderingRule DateOrdering { get; set; } = DateOrderingRule.BeginningOfExtent;

        [Parameter(Mandatory = false)]
        public LineLengthSettings LineLength { get; set; } = LineLengthSettings.Strict;

        private GedcomDatabase _database;

        protected override void BeginProcessing()
        {
            try
            {
                using (var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    GedcomSettings.LineLength = LineLength;
                    _database = new GedcomDatabase();
                    _database.Settings.AssumedDeathAge = AssumedDeathAge;
                    _database.Settings.DateOrderingRule = DateOrdering;
                    _database.Populate(fs);
                }
            }
            catch (GedcomReaderException grex)
            {
                _database = null;
                var error = new ErrorRecord(grex, "CannotReadGedcomFile", ErrorCategory.ReadError, Path);
                ThrowTerminatingError(error);
            }
            catch(Exception ex)
            {
                _database = null;
                var error = new ErrorRecord(ex, "CannotLoadGedcomFile", ErrorCategory.NotSpecified, Path);
                ThrowTerminatingError(error);
            }
        }

        protected override void ProcessRecord()
        {
            WriteObject(new PSGedcomDatabase(_database));
        }
    }
}