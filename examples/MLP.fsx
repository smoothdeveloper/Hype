﻿#r "../packages/FsAlg.0.5.11/lib/FsAlg.dll"
#r "../packages/DiffSharp.0.6.2/lib/DiffSharp.dll"
#I "../packages/RProvider.1.1.8"
#load "RProvider.fsx"

#load "../src/Hype/Hype.fs"
#load "../src/Hype/Optimize.fs"
#load "../src/Hype/Neural.fs"
#load "../src/Hype/Neural.MLP.fs"

open RDotNet
open RProvider
open RProvider.graphics

open FsAlg.Generic
open DiffSharp.AD
open Hype
open Hype.Neural

let OR = LabeledSet.create [[|D 0.; D 0.|], [|D 0.|]
                            [|D 0.; D 1.|], [|D 1.|]
                            [|D 1.; D 0.|], [|D 1.|]
                            [|D 1.; D 1.|], [|D 1.|]]

let XOR = LabeledSet.create [[|D 0.; D 0.|], [|D 0.|]
                             [|D 0.; D 1.|], [|D 1.|]
                             [|D 1.; D 0.|], [|D 1.|]
                             [|D 1.; D 1.|], [|D 0.|]]

let net = MLP.create([|2; 2; 1|])

let train (x:Vector<_>) =
    let par = {Params.Default with LearningRate = ScheduledLearningRate x; TrainFunction = Train.MSGD}
    let net2 = MLP.create([|2; 1|], Activation.sigmoid, D -0.05, D 0.05)
    let op = net2.Train par XOR
    op |> snd

let test2 = 
    let report i w _ =
        if i % 10 = 0 then
            namedParams [   
                "x", box (w |> Vector.map float |> Vector.toArray);
                "type", box "o"; 
                "col", box "blue";
                "ylim", box [0; 7]]
            |> R.plot |> ignore
    Optimize.GD {Params.Default with Epochs = 500; LearningRate = DecreasingLearningRate (D 0.1); GDReportFunction = report} train (Vector.create 200 (D 0.1))