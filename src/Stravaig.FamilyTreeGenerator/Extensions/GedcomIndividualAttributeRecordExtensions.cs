using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Stravaig.Gedcom.Model;

namespace Stravaig.FamilyTreeGenerator.Extensions;

public static class GedcomIndividualAttributeRecordExtensions
{
    // Key is regular occupation
    // Value is a list of top level occupations it can map to.
    private static ImmutableDictionary<string, string[]> _topLevelOccupations;

    private const string Baker = "Baker";
    private const string Blacksmith = "Blacksmith";
    private const string BusinessOwner = "Business Owner";
    private const string Carpenter = "Carpenter";
    private const string DomesticServant = "Domestic Servant";
    private const string Driver = "Driver";
    private const string EstateAgent = "Estate Agent";
    private const string FactoryWorker = "Factory Worker";
    private const string FarmWorker = "Farm Worker";
    private const string FishWorker = "Fish Worker";
    private const string Janitor = "Janitor";
    private const string Joiner = "Joiner";
    private const string Labourer = "Labourer";
    private const string Librarian = "Librarian";
    private const string Miner = "Miner";
    private const string Nurse = "Nurse";
    private const string Painter = "Painter";
    private const string Realtor = "Realtor";
    private const string RetailWorker = "Retail Worker";
    private const string Sales = "Sales";
    private const string Seaman = "Seaman";
    private const string Smith = "Smith";
    private const string Tailor = "Tailor";

    static GedcomIndividualAttributeRecordExtensions()
    {
        var builder = ImmutableDictionary.CreateBuilder<string, string[]>(StringComparer.InvariantCultureIgnoreCase);

        builder.Add("Able Seaman", new[] {Seaman});
        builder.Add("Agricultural Labourer", new[] {Labourer});
        builder.Add("Apprentice Blacksmith", new[] {Blacksmith});
        builder.Add("Assistant Librarian", new[] {Librarian});

        builder.Add("Baker (Journeyman)", new[] {Baker});
        builder.Add(Baker, new[] {Baker});
        builder.Add(Blacksmith, new[] {Blacksmith, Smith});
        builder.Add("Blacksmith Labourer", new[] {Blacksmith, Smith, Labourer});
        builder.Add("Boatswain", new[]{Seaman});
        builder.Add("Biscuit Factory Operative", new[]{FactoryWorker});
        
        builder.Add("Caretaker", new[]{Janitor});
        builder.Add(Carpenter, new[]{Carpenter, Joiner});
        builder.Add("Carter", new[]{Driver});
        builder.Add("Children's Nurse", new[] {Nurse});
        builder.Add("Clothing Business", new[]{BusinessOwner});
        builder.Add("Coachman (Domestic Servant)", new[]{Driver, DomesticServant});
        builder.Add("Coach Painter (Journeyman)", new[]{Painter});
        builder.Add("Coal Miner", new[]{Miner});
        builder.Add("Crane Driver", new []{Driver});
        
        builder.Add("Dairy Maid", new []{FarmWorker});
        builder.Add("Dairymaid", new []{FarmWorker});
        builder.Add("District Nurse", new[] {Nurse});
        builder.Add("Domestic", new[]{DomesticServant});
        builder.Add(DomesticServant, new[]{DomesticServant});
        
        builder.Add(FactoryWorker, new[]{FactoryWorker});
        builder.Add(FarmWorker, new[]{FarmWorker});
        builder.Add("Farm Servant", new[]{FarmWorker});
        builder.Add("Fish Salesman", new[]{Sales});
        builder.Add("Fishergirl", new[]{FishWorker});
        builder.Add("Fishwife", new[]{FishWorker});
        builder.Add("Fisherwomen", new[]{FishWorker});
        builder.Add(FishWorker, new[]{FishWorker});
        
        builder.Add("Ham Curer's Vanman", new[]{Driver});
        builder.Add("Head Janitor", new[]{Janitor});
        builder.Add("Housekeeper", new[]{DomesticServant});
        builder.Add("Houseworker", new[]{DomesticServant});
        
        builder.Add(Janitor, new[]{Janitor});
        builder.Add(Joiner, new[]{Joiner, Carpenter});
        
        builder.Add("Laborer", new[] {Labourer});
        builder.Add(Labourer, new[]{Labourer});
        builder.Add("Labourer at Colliery", new[]{Labourer, Miner});
        builder.Add("Ladies Outfitters Shop Assistant", new[]{Sales, RetailWorker});
        builder.Add(Librarian, new[]{Librarian});
        builder.Add("Library Attendant", new[]{Librarian});
        builder.Add("Light Duty Cycle Factory Warehouse", new[]{FactoryWorker});
        
        builder.Add("Mason's Labourer", new[]{Labourer});
        
        builder.Add("Office/Contract Cleaner", new[]{Janitor});
        
        builder.Add("Ploughman", new[]{FarmWorker});
        builder.Add("Practical Nursing", new[]{Nurse});
        builder.Add("Proprietor, Children's Nursery", new[]{BusinessOwner});
        
        builder.Add("Railway Driver", new[]{Driver});
        builder.Add("Railway Engine Driver", new[]{Driver});
        builder.Add("Real Estate Business", new[]{BusinessOwner, Realtor, EstateAgent});
        
        builder.Add("School Janitor", new[]{Janitor});
        builder.Add(Seaman, new[]{Seaman});
        builder.Add("Seaman (Merchant Services)", new[]{Seaman});
        
        builder.Add("Tailor (Master)", new[]{Tailor});
        builder.Add("Tinsmith", new[]{Smith});
        builder.Add("Tramcar Driver", new[]{Driver});

        _topLevelOccupations = builder.ToImmutable();
    }
    
    public static string[] TopLevelOccupationNames(this GedcomIndividualAttributeRecord occupationRecord)
    {
        if (occupationRecord.Tag != GedcomIndividualAttributeRecord.OccupationTag)
            return Array.Empty<string>();

        var occupationName = occupationRecord.Text;
        if (string.IsNullOrWhiteSpace(occupationName))
            return Array.Empty<string>();

        occupationName = occupationName.Trim();
        return _topLevelOccupations.TryGetValue(occupationName, out var topLevelNames)
            ? topLevelNames
            : new[] {occupationName};
    }
}