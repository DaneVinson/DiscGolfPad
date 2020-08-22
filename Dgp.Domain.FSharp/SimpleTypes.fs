namespace Dgp.Domain.Core

open System

type Result<'Success, 'Failure> = 
    | Success of 'Success
    | Failure of 'Failure


type EntityName = private EntityName of string

type Date = private Date of DateTimeOffset

type Location = private Location of string
    
type HoleDistance = private HoleDistance of int

type HoleNumber = private HoleNumber of int

type Id = private Id of Guid

type ImageUri = private ImageUri of string

type Par = private Par of int

type PlayerId = private PlayerId of string

type Score = private Score of int

type ScorecardNotes = private ScorecardNotes of string


module EntityName =
    
    // cannot be null or empty
    let create (name:string) = 
        if String.IsNullOrWhiteSpace(name) then
            Failure "The Name must have a value"
        else
            Success (EntityName name)

module Date =
    let create (date:DateTimeOffset) = 
        if true then
            Success (Date date)
        else
            Failure ""

module HoleDistance =
    
    // hole distance must be between 0 ft and 1 mile (5280 ft)
    let create (feet:int) =
        if feet < 0 || feet > 5280 then
            Failure "A hole distance must be between 1 foot and 1 mile (5280 ft)."
        else
            Success (HoleDistance feet)

module HoleNumber = 

    // hole number must be between 1 and 100
    let create (holeNumber:int) =
        if holeNumber < 1 || holeNumber > 100 then
            Failure "A hole mumber must be between 1 and 100"
        else
            Success (HoleNumber holeNumber)

module Id =
    
    // string must be a valid Guid
    let createFromString (id:string) =
        match Guid.TryParse id with
            | (true, guid) -> Success (Id guid)
            | _ -> Failure "The Id value is not valid"

module ImageUri =

    // null if not a valid uri
    let createFromString (uri:string) = 
        match Uri.TryCreate(uri, UriKind.RelativeOrAbsolute) with
            | (true, _uri) -> Success (ImageUri uri)
            | (false, _) -> Success (ImageUri "")
            | _ -> Failure ""

module Location =
    let create (location:string) = 
        if true then
            Success (Location location)
        else
            Failure ""

module Par = 

    // hole number must be between 1 and 20
    let create (par:int) =
        if par < 1 || par > 20 then
            Failure "Par must be between 1 and 20"
        else
            Success (Par par)

module PlayerId =
    
    // cannot be null or empty
    let create (id:string) = 
        if String.IsNullOrWhiteSpace(id) then
            Failure "Player Id must have a value"
        else
            Success (PlayerId id)

module Score = 

    // score must be between 1 and 50
    let create (score:int) =
        if score < 1 || score > 50 then
            Failure "The Score for a single hole must be between 1 and 50"
        else
            Success (Score score)

module ScorecardNotes =
    let create (notes:string) = 
        if true then
            Success (ScorecardNotes notes)
        else
            Failure ""
