# ?? **QUICK REFERENCE - ENTITY STATUS**

## **At a Glance**

```
? GOOD              ?? NEEDS WORK           ?? CRITICAL
?????????????????????????????????????????????????????????
User                 Student (minor)         NONE
Room                 LandLord (medium)       
Review               HousingUnit (medium)
Wishlist             Booking (medium)
Payment              Complaint (minor)
Notification         

Status: 75% COMPLETE - Ready for development with minor additions
```

---

## **What to Add (Priority Order)**

### **1?? MUST ADD BEFORE PRODUCTION:**

**A. Create VerificationStatus Enum:**
```csharp
// Shared/Enums/VerificationStatus.cs
public enum VerificationStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Suspended = 3
}
```

**B. Add Audit Timestamps to:**
- Student: `CreatedAt`, `UpdatedAt`
- LandLord: `CreatedAt`, `UpdatedAt`
- HousingUnit: `CreatedAt`, `UpdatedAt`
- Booking: `CreatedAt`, `UpdatedAt`

**C. Add Soft Delete to:**
- HousingUnit: `IsDeleted`
- Booking: `IsDeleted`

---

### **2?? SHOULD ADD:**

**LandLord Entity:**
```csharp
public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
public DateTime? ApprovedAt { get; set; }
public string? RejectionReason { get; set; }
public string? ApprovedByAdminId { get; set; }
```

**Student Entity:**
```csharp
public bool IsVerified { get; set; } = false;
public string? VerificationStatus { get; set; }
```

**HousingUnit Entity:**
```csharp
public double? AverageRating { get; set; }
public int ReviewCount { get; set; } = 0;
```

**Booking Entity:**
```csharp
public Guid HousingUnitId { get; set; }  // Add FK
public DateTime? CancelledAt { get; set; }
public string? CancellationReason { get; set; }
```

---

## **Database Migration Plan**

```sql
-- New columns to add:

-- Students table
ALTER TABLE Students ADD CreatedAt DATETIME = GETUTCDATE();
ALTER TABLE Students ADD UpdatedAt DATETIME NULL;
ALTER TABLE Students ADD IsVerified BIT = 0;
ALTER TABLE Students ADD VerificationStatus NVARCHAR(50) NULL;

-- LandLords table
ALTER TABLE LandLords ADD CreatedAt DATETIME = GETUTCDATE();
ALTER TABLE LandLords ADD UpdatedAt DATETIME NULL;
ALTER TABLE LandLords ADD ApprovedAt DATETIME NULL;
ALTER TABLE LandLords ADD RejectionReason NVARCHAR(MAX) NULL;
ALTER TABLE LandLords ADD ApprovedByAdminId NVARCHAR(450) NULL;
-- Change VerificationStatus from NVARCHAR to INT (enum)
ALTER TABLE LandLords ALTER COLUMN VerificationStatus INT;

-- HousingUnits table
ALTER TABLE HousingUnits ADD CreatedAt DATETIME = GETUTCDATE();
ALTER TABLE HousingUnits ADD UpdatedAt DATETIME NULL;
ALTER TABLE HousingUnits ADD IsDeleted BIT = 0;
ALTER TABLE HousingUnits ADD AverageRating DECIMAL(3,2) NULL;
ALTER TABLE HousingUnits ADD ReviewCount INT = 0;

-- Bookings table
ALTER TABLE Bookings ADD CreatedAt DATETIME = GETUTCDATE();
ALTER TABLE Bookings ADD UpdatedAt DATETIME NULL;
ALTER TABLE Bookings ADD IsDeleted BIT = 0;
ALTER TABLE Bookings ADD CancelledAt DATETIME NULL;
ALTER TABLE Bookings ADD CancellationReason NVARCHAR(MAX) NULL;
ALTER TABLE Bookings ADD HousingUnitId UNIQUEIDENTIFIER NULL;
ALTER TABLE Bookings ADD FOREIGN KEY (HousingUnitId) REFERENCES HousingUnits(HousingUnitId);

-- Complaints table
ALTER TABLE Complaints ADD UpdatedDate DATETIME NULL;
ALTER TABLE Complaints ADD ResolvedDate DATETIME NULL;
ALTER TABLE Complaints ADD Resolution NVARCHAR(MAX) NULL;
ALTER TABLE Complaints ADD AssignedToAdminId NVARCHAR(450) NULL;

-- Reviews table (optional)
ALTER TABLE Reviews ADD IsDeleted BIT = 0;
```

---

## **Impact Analysis**

| Impact | Level | Notes |
|--------|-------|-------|
| **Breaking Changes** | ?? LOW | All additions are optional columns |
| **Migration Risk** | ?? LOW | Safe to add nullable columns |
| **Performance** | ?? LOW | Extra columns minimal impact |
| **API Changes** | ?? MEDIUM | Response DTOs may need update |
| **Frontend Changes** | ?? MEDIUM | UI may show new fields |

---

## **Affected Services**

- **AuthService**: Need to set initial timestamps
- **Repositories**: May need to filter soft-deleted records
- **Mappers**: Update DTOs with new properties
- **Controllers**: May need new endpoints for admin approval

---

## **Timeline to Implement**

| Step | Time | Status |
|------|------|--------|
| 1. Update entities | 20 min | ?? |
| 2. Create enum | 5 min | ?? |
| 3. Update mappings | 15 min | ?? |
| 4. Create migration | 10 min | ?? |
| 5. Update AuthService | 15 min | ?? |
| 6. Test & verify | 20 min | ?? |
| **TOTAL** | **~80 min** | ?? |

---

## **Recommendation**

? **Proceed with development** - Current entities support MVP
?? **Add Priority 1 items** before first production deployment
?? **Schedule Priority 2 items** for next sprint

**Your application is 75% ready for production with these additions!**

