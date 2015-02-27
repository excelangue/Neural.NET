﻿namespace NeuralNet
open MathNet.Numerics.LinearAlgebra

module private Error =

    // Final layer error calulation (need to make work for cross-entropy)
    let OutputError(activation, actPrime, partialCost, lastZ : Vector<double>, y) =
        partialCost(actPrime, lastZ, lastZ.Map(activation, Zeros.Include), y)

    // Error of layer before
    let PreviousError(actPrime, nextError : Vector<double>, z : Vector<double>, nextWeight : Matrix<double>) =
        let weightedError = Matrix.op_Multiply(nextWeight.Transpose(), nextError)
        Vector.op_DotMultiply(weightedError, z.Map(actPrime, Zeros.Include))

    // Error of entire network
    let NetworkError(activation, actPrime, partialCost, reverseZ : Vector<double> list, y, w : Matrix<double> list) : Vector<double> list =
        let reverseW = List.rev(w)

        let rec stepBack(nextErrors : Vector<double> list, reverseZ : Vector<double> list, reverseW : Matrix<double> list) : Vector<double> list = 
            if reverseZ.Length = 1 then //don't care about "error" of input
                nextErrors
            else
            let nextErrors' = List.Cons(PreviousError(actPrime, nextErrors.Head, reverseZ.Head, reverseW.Head), nextErrors)
            stepBack(nextErrors', reverseZ.Tail, reverseW.Tail)

        stepBack([OutputError(activation, actPrime, partialCost, reverseZ.Head, y)], reverseZ.Tail, reverseW)

    let Backpropagate(activation, actPrime, partialCost, a, y, w : Matrix<double> list, b) : Matrix<double> list * Vector<double> list =
        let reverseZ = NeuralNet.Output.FeedForward(activation, [a], w, b)
        let error = NetworkError(activation, actPrime, partialCost, reverseZ, y, w)

        let nablaW = [for i in 0 .. error.Length - 1 do yield (error.[i].ToColumnMatrix() * Matrix.transpose(reverseZ.[(reverseZ.Length - 1) - i].Map(activation, Zeros.Include).ToColumnMatrix()))]

        (nablaW, error)
