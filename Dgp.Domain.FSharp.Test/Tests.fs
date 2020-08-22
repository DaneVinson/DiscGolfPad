module Tests

open System
open Xunit
open Dgp.Domain
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Dgp.Domain.Core

[<Theory>]
[<InlineData(null)>]
[<InlineData("")>]
[<InlineData(" ")>]
[<InlineData("somename")>]
let ``EntityName is required`` (name:string) =
    // arrange
    let expectedSuccess = not (String.IsNullOrWhiteSpace(name))

    // act
    let success = 
        match EntityName.create(name) with
            | Success n -> true
            | Failure msg -> false

    // assert
    Assert.Equal(expectedSuccess, success)

[<Fact>]
let ``Stupid Test`` () =

    let courseDto = {
        PlayerId = "dane"
        Id = Guid.NewGuid().ToString()
        Name = "Course Foo" 
        Location = "Bar Land" 
        ImageUri = "" 
        HolePars = [|3; 3; 3;|] 
        HoleDistances = [|250; 260; 270;|]
    }

    let json = "{\"holes\":9,\"id\":\"66321341-b59e-4d5e-84e1-aa5e6e5187ec\",\"name\":\"Orchard Park\",\"par\":27,\"playerId\":\"E593FEE6-BB9D-4118-8936-4F5A03820F85\"}"

    let x = JsonConvert.DeserializeObject<CourseInfo> json

    let course = Course.create courseDto

    Assert.True(true)
