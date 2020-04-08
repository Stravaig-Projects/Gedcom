# Stravaig.Gedcom

A GEDCOM file reader

## Build Status

![Build Stravaig.Gedcom](https://github.com/Stravaig-Projects/Gedcom/workflows/Build%20Stravaig.Gedcom/badge.svg)

## Support

### Dates

Can parse types:
* Date
* Date Period
* Date Range
* Date Approximated
* Interpreted Date
* Date Phrase

Can parse:
* Gregorian
* Julian

Not supported:
* Years prior to 32 CE
* `(B.C.)` year suffix to denote years BCE.
* The slash "/" <DIGIT><DIGIT> a year modifier which shows the possible date alternatives.
* Date conversion (e.g. Julian to Gregorian)

Supported, but not standard
* Single digit days (spec says `dd`)
