[<AutoOpen>]
module Helpers

open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open System.IO
open System.Net.Http
open System.Text
open NSubstitute


let getEmptyContext (method: string) (path : string) =
  let ctx = Substitute.For<HttpContext>()
  ctx.Request.Method.ReturnsForAnyArgs method |> ignore

  ctx.Request.Scheme.ReturnsForAnyArgs ("http") |> ignore
  ctx.Request.Host.ReturnsForAnyArgs (HostString("localhost")) |> ignore
  ctx.Request.PathBase.ReturnsForAnyArgs(PathString "") |> ignore
  ctx.Request.Path.ReturnsForAnyArgs (PathString(path)) |> ignore
  ctx.Request.QueryString.ReturnsForAnyArgs (QueryString "") |>ignore

  ctx.Response.Body <- new MemoryStream()
  ctx

let next : HttpFunc = Some >> Task.FromResult

let runTask task =
  task
  |> Async.AwaitTask
  |> Async.RunSynchronously

let getContentType (response : HttpResponse) =
  response.Headers.["Content-Type"].[0]

let getStatusCode (ctx : HttpContext) =
  ctx.Response.StatusCode

let getBody (ctx : HttpContext) =
  ctx.Response.Body.Position <- 0L
  use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
  reader.ReadToEnd()

let readText (response : HttpResponseMessage) =
  response.Content.ReadAsStringAsync()
  |> runTask

let readBytes (response : HttpResponseMessage) =
  response.Content.ReadAsByteArrayAsync()
  |> runTask