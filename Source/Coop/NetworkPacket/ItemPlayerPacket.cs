﻿namespace StayInTarkov.Coop.NetworkPacket
{
    public class ItemPlayerPacket : BasePlayerPacket
    {
        public string ItemId { get; set; }

        public string TemplateId { get; set; }

        public ItemPlayerPacket(string profileId, string itemId, string templateId, string method)
            : base(new string(profileId.ToCharArray()), method)
        {
            ItemId = itemId;
            TemplateId = templateId;
        }
    }
}