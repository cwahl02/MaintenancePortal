using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for seeding initial user and ticket data into the application's database for development or testing
/// purposes.
/// </summary>
/// <remarks>This class is typically used during application startup to ensure that the database contains a set of
/// sample users and tickets. It is intended for use in development or test environments and should not be used to seed
/// production data.</remarks>
public class SeedData
{
    /// <summary>
    /// Asynchronously seeds the database with initial user and ticket data if it does not already exist.
    /// </summary>
    /// <remarks>This method should be called during application startup to ensure that required users and
    /// tickets are present in the database. It is safe to call multiple times; existing data will not be
    /// duplicated.</remarks>
    /// <param name="context">The database context used to access and modify ticket data.</param>
    /// <param name="userManager">The user manager used to create and manage user accounts during seeding.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    public static async Task InitializeAsync(AppDbContext context, UserManager<User> userManager)
    {
        // Seed users first
        await SeedUsersAsync(userManager);

        // Seed tickets after users are created
        await SeedTicketsAsync(context);
    }

    /// <summary>
    /// Seeds the user store with a set of sample users if no users currently exist.
    /// </summary>
    /// <remarks>This method generates a predefined set of sample users with unique usernames and emails. It
    /// should be called during application initialization or testing to ensure that the user store contains sample
    /// data. The method does nothing if users already exist.</remarks>
    /// <param name="userManager">The user manager used to create and manage user accounts. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    /// <exception cref="Exception">Thrown if a user cannot be created successfully during the seeding process.</exception>
    private static async Task SeedUsersAsync(UserManager<User> userManager)
    {
        // Check if users already exist
        if (!userManager.Users.Any())
        {
            // List of sample first names and last names
            var firstNames = new List<string> { "John", "Jane", "Mary", "James", "Lisa", "Michael", "Sarah", "David", "Emily", "Charles", "Sophia", "Lucas", "Olivia", "Ethan", "Mia" };
            var lastNames = new List<string> { "Doe", "Smith", "Johnson", "Williams", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris" };

            // Generate users (total of 15 users)
            for (int i = 0; i < 15; i++)
            {
                var firstName = firstNames[new Random().Next(firstNames.Count)];
                var lastName = lastNames[new Random().Next(lastNames.Count)];

                var userName = $"{firstName.ToLower()}{lastName.ToLower()}"; // Username = firstname + lastname
                var email = $"{userName}@maintenanceportal.com"; // Email follows the same pattern
                var password = $"{firstName}{lastName}123!"; // Simple password pattern

                var user = new User
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    DisplayName = $"{firstName} {lastName}",
                    Bio = $"Hello, I'm {firstName} {lastName}, a user of Maintenance Portal.",
                };

                // Create the user
                IdentityResult result = await userManager.CreateAsync(user, password);
                if(result != IdentityResult.Success)
                {
                    throw new Exception($"Failed to create user {userName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    /// <summary>
    /// Seeds the database with sample ticket data if no tickets currently exist and at least one user is present.
    /// </summary>
    /// <remarks>If tickets already exist or no users are found in the database, the method does not add any
    /// tickets. This method is intended for development or testing scenarios to populate the database with initial
    /// ticket data.</remarks>
    /// <param name="context">The database context used to access and modify ticket and user data. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous seeding operation.</returns>
    private static async Task SeedTicketsAsync(AppDbContext context)
    {
        // Check if tickets already exist or users don't exist yet
        if (context.Tickets.Any())
        {
            Console.WriteLine("Tickets already exist. Skipping ticket seeding.");
            return;
        }

        var users = context.Users.ToList(); // Get all users from the database

        if (!users.Any()) // Check if users exist
        {
            Console.WriteLine("No users found. Skipping ticket seeding.");
            return;
        }

        Console.WriteLine($"Found {users.Count} users. Seeding tickets...");

        var tickets = new List<Ticket>();

        // Create 30 tickets (3 pages of 10 tickets)
        for (int i = 1; i <= 30; i++)
        {
            bool state = i % 3 == 0; // Rotate through TicketState values (e.g., Open, InProgress, Closed)

            // Make sure the CreatedById is valid (i.e., it must refer to a valid user)
            var createdByUser = users[new Random().Next(users.Count)];
            if (createdByUser == null)
            {
                Console.WriteLine($"Error: Could not find a valid user for CreatedById for Ticket {i}");
                continue; // Skip this ticket if the user is invalid
            }

            var ticket = new Ticket
            {
                Title = $"Ticket {i}: Issue {i}",
                Description = $"This is a description for issue {i}. More details can be found here.",
                CreatedById = createdByUser.Id, // Assign CreatedById to a valid user
                CreatedAt = DateTime.Now.AddDays(-new Random().Next(1, 10)), // Random creation date within the last 10 days
                IsOpen = state,
                LastModifiedAt = DateTime.Now.AddDays(-new Random().Next(0, 5)), // Random last modified date within the last 5 days
                ClosedAt = state == false ? DateTime.Now.AddDays(-new Random().Next(0, 3)) : null // Set ClosedAt only if state is Closed
            };

            tickets.Add(ticket);
        }

        // Log how many tickets we're adding to the database
        Console.WriteLine($"Adding {tickets.Count} tickets to the database...");

        // Add tickets to the database
        context.Tickets.AddRange(tickets);

        try
        {
            await context.SaveChangesAsync();
            Console.WriteLine("Tickets saved successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            // Log detailed error information if DbUpdateException occurs
            Console.WriteLine($"Error while saving tickets: {dbEx.Message}");
            Console.WriteLine("Inner Exception: " + dbEx.InnerException?.Message);
        }
        catch (Exception ex)
        {
            // Log any general errors
            Console.WriteLine($"Error while saving tickets: {ex.Message}");
        }
    }


}
