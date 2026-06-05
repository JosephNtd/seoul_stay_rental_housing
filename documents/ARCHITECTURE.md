# Projects

GUI_WPF
- View
- ViewModel

BUS
- Business Logic

DAL
- Database Access

DTO
- Data Transfer Objects
- LINQ

ET
- Entities / Constants

Helpers
- Shared Utilities

# Rules

GUI_WPF -> BUS
GUI_WPF -> DTO
GUI_WPF -> ET
BUS -> DAL
BUS -> DTO
DAL -> DTO
DAL -> ET

GUI không gọi DAL trực tiếp.
BUS không tham chiếu GUI.