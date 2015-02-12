﻿namespace Network
open MathNet.Numerics.LinearAlgebra

module Costs =
    type Quadratic =
        static member Cost(a : Vector<double>, y : Vector<double>) : double =
            (y - a).Norm(2.0) ** 2.0
        static member PartialCost (a : Vector<double>, y : Vector<double>) : Vector<double> =
            (a - y)