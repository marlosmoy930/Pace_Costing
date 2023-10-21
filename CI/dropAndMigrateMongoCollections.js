function HexToBase64(hex) {
    var base64Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    var base64 = "";
    var group;
    for (var i = 0; i < 30; i += 6) {
        group = parseInt(hex.substr(i, 6), 16);
        base64 += base64Digits[(group >> 18) & 0x3f];
        base64 += base64Digits[(group >> 12) & 0x3f];
        base64 += base64Digits[(group >> 6) & 0x3f];
        base64 += base64Digits[group & 0x3f];
    }
    group = parseInt(hex.substr(30, 2), 16);
    base64 += base64Digits[(group >> 2) & 0x3f];
    base64 += base64Digits[(group << 4) & 0x3f];
    base64 += "==";
    return base64;
}

function Base64ToHex(base64) {
    var base64Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    var hexDigits = "0123456789abcdef";
    var hex = "";
    for (var i = 0; i < 24; ) {
        var e1 = base64Digits.indexOf(base64[i++]);
        var e2 = base64Digits.indexOf(base64[i++]);
        var e3 = base64Digits.indexOf(base64[i++]);
        var e4 = base64Digits.indexOf(base64[i++]);
        var c1 = (e1 << 2) | (e2 >> 4);
        var c2 = ((e2 & 15) << 4) | (e3 >> 2);
        var c3 = ((e3 & 3) << 6) | e4;
        hex += hexDigits[c1 >> 4];
        hex += hexDigits[c1 & 15];
        if (e3 != 64) {
            hex += hexDigits[c2 >> 4];
            hex += hexDigits[c2 & 15];
        }
        if (e4 != 64) {
            hex += hexDigits[c3 >> 4];
            hex += hexDigits[c3 & 15];
        }
    }
    return hex;
}

function UUID(uuid) {
    var hex = uuid.replace(/[{}-]/g, ""); // remove extra characters
    var base64 = HexToBase64(hex);
    return new BinData(4, base64); // new subtype 4
}

function CSUUID(uuid) {
    var hex = uuid.replace(/[{}-]/g, ""); // remove extra characters
    var a = hex.substr(6, 2) + hex.substr(4, 2) + hex.substr(2, 2) + hex.substr(0, 2);
    var b = hex.substr(10, 2) + hex.substr(8, 2);
    var c = hex.substr(14, 2) + hex.substr(12, 2);
    var d = hex.substr(16, 16);
    hex = a + b + c + d;
    var base64 = HexToBase64(hex);
    return new BinData(3, base64);
}

BinData.prototype.toUUID = function () {
    var hex = Base64ToHex(this.base64()); // don't use BinData's hex function because it has bugs in older versions of the shell
    var uuid = hex.substr(0, 8) + '-' + hex.substr(8, 4) + '-' + hex.substr(12, 4) + '-' + hex.substr(16, 4) + '-' + hex.substr(20, 12);
    return 'UUID("' + uuid + '")';
}

BinData.prototype.toCSUUID = function () {
    var hex = Base64ToHex(this.base64()); // don't use BinData's hex function because it has bugs in older versions of the shell
    var a = hex.substr(6, 2) + hex.substr(4, 2) + hex.substr(2, 2) + hex.substr(0, 2);
    var b = hex.substr(10, 2) + hex.substr(8, 2);
    var c = hex.substr(14, 2) + hex.substr(12, 2);
    var d = hex.substr(16, 16);
    hex = a + b + c + d;
    var uuid = hex.substr(0, 8) + '-' + hex.substr(8, 4) + '-' + hex.substr(12, 4) + '-' + hex.substr(16, 4) + '-' + hex.substr(20, 12);
    return 'CSUUID("' + uuid + '")';
}

BinData.prototype.toHexUUID = function () {
    var hex = Base64ToHex(this.base64()); // don't use BinData's hex function because it has bugs
    var uuid = hex.substr(0, 8) + '-' + hex.substr(8, 4) + '-' + hex.substr(12, 4) + '-' + hex.substr(16, 4) + '-' + hex.substr(20, 12);
    return 'HexData(' + this.subtype() + ', "' + uuid + '")';
}

//Migrate BidPricePerfomance
print("Migrating BidPricePerformace");
db.getCollectionNames().forEach(function(item, i, array) {
  var pattern = /^BidPricePerfomance_([0-9]+)$/;
  var match = pattern.exec(item);
  if (match !== null) {
    var bidId = parseInt(match[1]);
    print("Migrating and dropping " + item);
    db[item].find().forEach(function(pr) {
      try {
        db.BidPricePerformance.insertOne({
          _id: pr.cgId,
          bidId: NumberInt(bidId),
          sYear: NumberInt(pr.sYear),
          sMonth: NumberInt(pr.sMonth),
          elements: pr.elements
        });
      } catch (e) {
        print(
          "BidPricePerformance Duplicate. Skip(possibly was an error) BidId: " +
            bidId +
            " . CostGroupId: " +
            pr.cgId.hex()
        );
      }
    });
    db[item].drop();
  }
});

//Migrate BidInflation
print("Migrating BidInflation");
db.getCollectionNames().forEach(function(item, i, array) {
  var pattern = /^BidInflation_([0-9]+)$/;
  var match = pattern.exec(item);
  if (match !== null) {
    var bidId = parseInt(match[1]);
    print("Migrating and dropping " + item);
    db[item].find().forEach(function(pr) {
      try {
        db.BidInflation.insertOne({
          _id: pr.cgId,
          bidId: NumberInt(bidId),
          sYear: NumberInt(pr.sYear),
          sMonth: NumberInt(pr.sMonth),
          elements: pr.elements
        });
      } catch (e) {
        print(
          "BidInflation Duplicate. Skip(possibly was an error) BidId: " +
            bidId +
            " . CostGroupId: " +
            pr.cgId.hex()
        );
      }
    });
    db[item].drop();
  }
});

print("Rename valuesWoCola to valuesCola");
try {
  var updateModels = [];
  db.BidInflation.find({"elements.valuesWoCola": { $exists: true }}).forEach(function(item) {
    if (item.elements.some(x => x.valuesWoCola)) {
    var newElements = [];
    item.elements.forEach(function(element) {
      newElements.push({
      eId: element.eId,
      adjValue: element.adjValue,
      values: element.values,
      valuesCola: element.valuesWoCola
      });
    });

    item.elements = newElements;

    updateModels.push({
      updateOne: {
      "filter": {
        "bidId": item.bidId,
        "_id": item._id
      },
      "update": item
      }
    });
    }
  });
  db.BidInflation.bulkWrite(updateModels, {
    ordered: false
  });
} catch (e) {
  print("error while renaming valuesWoCola to valuesColas ..." + e);
}

//Migrate customStructureSummary
print("Migrating customStructureSummary");
db.getCollectionNames().forEach(function(item, i, array) {
    var pattern = /^customStructureSummary_([0-9]+)_([0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12})$/;
    var match = pattern.exec(item);
    if (match !== null) {
      db[item].drop();
    }
  });

//Migrate defaultStructureSummary
print("Migrating defaultStructureSummary");
db.getCollectionNames().forEach(function(item, i, array) {
    var pattern = /^defaultStructureSummary_([0-9]+)_([0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12})$/;
    var match = pattern.exec(item);
    if (match !== null) {
      db[item].drop();
    }
  });

//drop reports
print("dropping WBS reports");
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("WbsReport_") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping WBS reports ..." + e);
}

print("dropping UnitCost reports");
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("UnitCostReport_") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping UnitCost reports ..." + e);
}

print("dropping Bnm reports");
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("BnmReport_") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping BnmReport reports ..." + e);
}

//drop CalculatedCostAttrubutes_{bidId}
print("dropping CalculatedCostAttrubutes collections");
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("CalculatedCostAttrubutes_") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping CalculatedCostAttrubutes ..." + e);
}

//drop all Quantity collections with guids (CalculatedQuantities_{guid})
print("dropping CalculatedQuantities collections");
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("CalculatedQuantities_") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping CalculatedQuantities ..." + e);
}

//drop all CostAttributesCalculatedData(calculation service)
print("dropping CostAttributesCalculatedData collections");
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("CostAttributesCalculatedData_") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping CostAttributesCalculatedData ..." + e);
}

// R2.1 Migrations

//drop CostAttributesCalculatedData to clear all indexes and duplicates
try {
  db.getCollectionNames().forEach(function(c) {
    if (c.indexOf("CostAttributesCalculatedData") == 0) db[c].drop();
  });
} catch (e) {
  print("error while dropping CostAttributesCalculatedData ..." + e);
}

try {
    db.BidPricePerformance.drop();
} catch (e) {
    print("error while dropping BidPricePerformance ..." + e);
}