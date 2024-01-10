// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         RNGSelector.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/16/2023 2:48 PM
//    Created Date:     10/16/2023 2:48 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoEvent.Games.Glass.Features;
using AutoEvent.Games.Infection;
using HarmonyLib;

namespace AutoEvent.API.RNG;

public class RNGGenerator
{
    public static int GetIntFromSeededString(string seed, int count, int offset, int amount)
    {
        string seedGen = "";
        for (int s = 0; s < count; s++)
        {
            int indexer = (amount * count) + s;
            while (indexer >= seed.Length)
                indexer -= seed.Length - 1;
            seedGen += seed[indexer].ToString();
        }

        return int.Parse(seedGen);
    }

    public static byte[] GetRandomBytes()
    {
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        byte[] rand = new byte[8];
        rng.GetBytes(rand);
        return rand;
    }

    public static byte[] TextToBytes(string inputText)
    {
        byte[] rand = Encoding.ASCII.GetBytes(inputText);
        return rand;
    }

    public static string GetSeed(byte[] seed)
    {

        var sha = SHA256.Create();
        byte[] bytes = sha.ComputeHash(seed); // System.Text.Encoding.ASCII.GetBytes(seed));

        string newSeed = "";
        foreach (byte bytemap in bytes)
        {
            newSeed += bytemap.ToString();
        }

        return newSeed;
    }

    public static int GetRandomNumber(int minValue, int maxValue)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] randomNumber = new byte[4];
            rng.GetBytes(randomNumber);

            int number = BitConverter.ToInt32(randomNumber, 0);

            if (minValue == maxValue)
            {
                return minValue;
            }

            number = Math.Abs(number % (maxValue - minValue + 1));
            number += minValue;

            return number;
        }
    }
}