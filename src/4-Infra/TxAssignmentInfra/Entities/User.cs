﻿namespace TxAssignmentInfra.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
