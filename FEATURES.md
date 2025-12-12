# Features & Roadmap

This document outlines the current features and planned enhancements for MaintenancePortal.

---

## Feature Status Legend
- ? **Complete** - Feature is fully implemented
- ?? **In Progress** - Feature is currently being developed
- ? **Planned** - Feature is planned for future development

---

## Database Schema

- [x] Database initialization using Entity Framework Core Migrations
   - [x] Tickets Table
   - [x] Users Table
   - [ ] Tags Table
   - [ ] Comments Table
   - [ ] Jobs Table
   - [ ] Permission Sets Table
   - [ ] Groups Table
   - [ ] Audit Logs Table

---

## Core Features

### ? Ticket System (Complete)
- [x] Create tickets
- [x] View ticket details
- [x] Edit tickets
- [x] Delete tickets

### ? User System (In Progress)
- [x] User registration
- [x] User login
- [x] Edit user profile
- [ ] Delete user account

### ?? Tag System
- [ ] Create tags
- [ ] Edit tags
- [ ] Delete tags
- [ ] Assign tags to tickets

### ? Comment System
- [ ] Add comments to tickets
- [ ] Edit comments
- [ ] Delete comments

### ? Group System
- [ ] Create groups
- [ ] Edit groups
- [ ] Delete groups
- [ ] Add/remove users from groups
- [ ] Assign tickets to groups

---

## Advanced Features

### ?? Advanced Ticket Management
- [ ] Ticket assignment to users
- [x] Ticket status tracking (Open, In Progress, Completed)
- [ ] Ticket priority levels
- [ ] Ticket filtering and search
- [ ] Ticket tags (GitHub issue labels style)
   - [ ] Color-coded labels
   - [ ] Multiple tags per ticket
   - [ ] Filter tickets by tags
   - [ ] Permission-based access/viewing
- [ ] Group-based tickets
   - [ ] Assign tickets to groups
   - [ ] Group members can view group tickets
   - [ ] Group-based ticket filtering
   - [ ] Group ticket permissions

### ?? Commenting System
- [ ] Add comments to tickets
- [ ] View comment history
- [ ] Edit own comments
- [ ] Delete own comments
- [ ] Admin delete any comment

### ? User Dashboard
- [ ] Overview of assigned tickets
- [ ] Recent activity feed
- [ ] Ticket statistics (Open, In Progress, Completed)
- [ ] Quick actions (Create ticket, View profile)
- [ ] Upcoming deadlines/priorities
- [ ] Group tickets overview

### ?? User Profile & Settings
- [ ] Advanced user profile
   - [ ] Profile picture upload
   - [ ] Change display name
   - [ ] Bio/description
   - [ ] Activity history
   - [ ] Assigned tickets view
- [ ] Settings page
   - [ ] Change password
   - [ ] Change email
   - [ ] Change username
   - [ ] Account security settings
   - [ ] Disable account
   - [ ] Delete account

### ? Jobs and Permission System
- [ ] Job titles management
   - [ ] Create job titles
   - [ ] Assign job titles to users
   - [ ] Multiple job titles per user
   - [ ] Display-only (not for access control)
- [ ] Permission sets
   - [ ] Create permission sets
   - [ ] Assign permissions to sets
   - [ ] Assign permission sets to users/groups
   - [ ] Granular access control (Create, Read, Update, Delete permissions)
- [ ] Groups
   - [ ] Create groups
   - [ ] Add/remove users from groups
   - [ ] Group-based permissions via permission sets
   - [ ] Group-specific ticket visibility
   - [ ] Potential group isolation (instance-like behavior)

### ? Audit Log System
- [ ] Log all user actions
   - [ ] Ticket creation, updates, deletion
   - [ ] User account changes
   - [ ] Login/logout events
   - [ ] Permission changes
   - [ ] Group modifications
- [ ] Audit log viewing
   - [ ] Permission-based access to view logs
   - [ ] Users can view their own activity
   - [ ] Admins can view all activity
   - [ ] Filter logs by user, action type, date range
   - [ ] Search audit logs
- [ ] Audit log retention policies

### ? Theme System
- [ ] Light theme
- [ ] Dark theme
- [ ] Custom color schemes
- [ ] User preference persistence

### ? Notification System
- [ ] Email notifications for ticket updates
- [ ] In-app notifications
- [ ] Notification for group ticket updates

### ? Reporting and Analytics
- [ ] Ticket completion statistics
- [ ] User performance metrics
- [ ] Group performance metrics
- [ ] Export reports to PDF/CSV
