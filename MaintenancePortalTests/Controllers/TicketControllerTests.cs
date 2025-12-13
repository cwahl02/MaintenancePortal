using MaintenancePortal.Data;
using MaintenancePortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaintenancePortal.Controllers.Tests
{
    [TestClass]
    public class TicketControllerTests
    {
        private AppDbContext _context;
        private TicketController _controller;
        private User _user;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            _user = new User
            {
                Id = "user1",
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                DisplayName = "Test User",
                Birthdate = DateTime.Now.AddYears(-30),
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(_user);
            _context.SaveChanges();

            _controller = new TicketController(_context);
        }

        private void SeedTickets(int openCount, int closedCount)
        {
            var tickets = new List<Ticket>();
            for (int i = 0; i < openCount; i++)
            {
                tickets.Add(new Ticket
                {
                    Title = $"Open Ticket {i + 1}",
                    Description = $"This is open ticket {i + 1}.",
                    IsOpen = true,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now,
                    CreatedById = _user.Id
                });
            }
            for (int i = 0; i < closedCount; i++)
            {
                tickets.Add(new Ticket
                {
                    Title = $"Closed Ticket {i + 1}",
                    Description = $"This is closed ticket {i + 1}.",
                    IsOpen = false,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now,
                    ClosedAt = DateTime.Now,
                    CreatedById = _user.Id
                });
            }
            _context.Tickets.AddRange(tickets);
            _context.SaveChanges();
        }

        [TestMethod]
        [DataRow(3, 2, 1, 10)]
        [DataRow(0, 3, 1, 10)]
        [DataRow(5, 23, 1, 10)]
        public async Task Index_ReturnsAllTickets(
            int openTickets,
            int closedTickets,
            int page,
            int pageSize)
        {
            // Arrange
            SeedTickets(openTickets, closedTickets);
            int totalCount = openTickets + closedTickets;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            int expectedTicketsCount = Math.Min(pageSize, totalCount - (page - 1) * pageSize);

            // Act
            var result = await _controller.Index(null, page, pageSize);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as TicketPaginationViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(totalCount, model.TotalTickets);
            Assert.AreEqual(openTickets, model.TotalOpenTickets);
            Assert.AreEqual(closedTickets, model.TotalClosedTickets);
            Assert.AreEqual(totalPages, model.TotalPages);
            Assert.AreEqual(page, model.CurrentPage);
            Assert.AreEqual(expectedTicketsCount, model.Tickets.Count);
        }

        [TestMethod]
        [DataRow(3, 2, 1, 10)]
        [DataRow(0, 3, 1, 10)]
        [DataRow(5, 23, 1, 10)]
        [DataRow(7, 5, 2, 4)]
        public async Task Index_ReturnsOpenTickets(
            int openTickets,
            int closedTickets,
            int page,
            int pageSize)
        {
            // Arrange
            SeedTickets(openTickets, closedTickets);
            int filteredCount = openTickets;
            int totalPages = (int)Math.Ceiling((double)filteredCount / pageSize);
            int expectedTicketsCount = Math.Min(pageSize, Math.Max(0, filteredCount - (page - 1) * pageSize));

            // Act
            var result = await _controller.Index(true, page, pageSize);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as TicketPaginationViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(openTickets + closedTickets, model.TotalTickets);
            Assert.AreEqual(openTickets, model.TotalOpenTickets);
            Assert.AreEqual(closedTickets, model.TotalClosedTickets);
            Assert.AreEqual(totalPages, model.TotalPages);
            Assert.AreEqual(page, model.CurrentPage);
            Assert.AreEqual(expectedTicketsCount, model.Tickets.Count);
            Assert.IsTrue(model.Tickets.All(t => t.IsOpen));
        }

        [TestMethod]
        [DataRow(3, 2, 1, 10)]
        [DataRow(0, 3, 1, 10)]
        [DataRow(5, 23, 1, 10)]
        [DataRow(7, 5, 2, 4)]
        public async Task Index_ReturnsClosedTickets(
            int openTickets,
            int closedTickets,
            int page,
            int pageSize)
        {
            // Arrange
            SeedTickets(openTickets, closedTickets);
            int filteredCount = closedTickets;
            int totalPages = (int)Math.Ceiling((double)filteredCount / pageSize);
            int expectedTicketsCount = Math.Min(pageSize, Math.Max(0, filteredCount - (page - 1) * pageSize));

            // Act
            var result = await _controller.Index(false, page, pageSize);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as TicketPaginationViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(openTickets + closedTickets, model.TotalTickets);
            Assert.AreEqual(openTickets, model.TotalOpenTickets);
            Assert.AreEqual(closedTickets, model.TotalClosedTickets);
            Assert.AreEqual(totalPages, model.TotalPages);
            Assert.AreEqual(page, model.CurrentPage);
            Assert.AreEqual(expectedTicketsCount, model.Tickets.Count);
            Assert.IsTrue(model.Tickets.All(t => !t.IsOpen));
        }

        [TestMethod]
        [DataRow(7, 5, 2, 4)]
        [DataRow(10, 10, 3, 7)]
        public async Task Index_ReturnsCorrectPagination(
            int openTickets,
            int closedTickets,
            int page,
            int pageSize)
        {
            // Arrange
            SeedTickets(openTickets, closedTickets);
            int totalTickets = openTickets + closedTickets;
            int expectedTotalPages = (int)Math.Ceiling(totalTickets / (double)pageSize);
            int expectedTicketsOnPage = Math.Min(pageSize, Math.Max(0, totalTickets - (page - 1) * pageSize));

            // Act
            var result = await _controller.Index(null, page, pageSize);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as TicketPaginationViewModel;
            Assert.IsNotNull(model);

            Assert.AreEqual(expectedTotalPages, model.TotalPages);
            Assert.AreEqual(page, model.CurrentPage);
            Assert.AreEqual(expectedTicketsOnPage, model.Tickets.Count);
        }
    }
}