﻿using System.Text.Json.Serialization;

namespace FireEscape.Models
{
    public partial class UserAccount : ObservableObject
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        [ObservableProperty]
        string? id;
        [ObservableProperty]
        string? name;
        [ObservableProperty]
        DateTime? expirationDate;

        [ObservableProperty]
        List<string> roles = new();

        [JsonIgnore]
        public bool IsAdmin
        {
            get => Roles.Contains(AdminRole);
            set
            {
                if (IsAdmin != value)
                {
                    if (value)
                        Roles.Add(AdminRole);
                    else
                        Roles.Remove(AdminRole);
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }
    }
}