// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AutoEvent
//    Project:          AutoEvent
//    FileName:         Weapon.cs
//    Author:           Redforce04#4091
//    Revision Date:    10/02/2023 10:11 PM
//    Created Date:     10/02/2023 10:11 PM
// -----------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using YamlDotNet.Serialization;

namespace AutoEvent.API;

public class Weapon : Item
{
    public Weapon() { }
    public Weapon(ItemType itemType)
    {
        this.ItemType = itemType;
        Attachments = new List<Attachment>();
    }

    public Weapon(ItemType itemType, List<Attachment> attachments)
    {
        ItemType = itemType;
        Attachments = attachments;
    }
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitNull |
                                        DefaultValuesHandling.OmitEmptyCollections)]
    [Description("A list of attachments that the weapon should have.")]
    public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    
    public override bool Equals(object obj)
    {
        if (obj is ItemType type)
        {
            if (type == ItemType)
                return true;
            return false;
        }

        return false;
    }

    public override string ToString()
    {
        string attachments = ",";
        foreach (Attachment attachment in Attachments)
        {
            attachments += $", {attachment}";
        }

        if (attachments == ",")
            attachments = "";
        else
            attachments = attachments.Replace(",, ", "");
        return $"{ItemType} [{attachments}]";
    }
}