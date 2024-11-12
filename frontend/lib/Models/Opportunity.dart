import 'package:flutter/foundation.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Models/OpportunityImg.dart';

class Opportunity {
  final int opportunityId;
  final String name;
  final double price;
  final int vacancies;
  final bool isActive;
  final OppCategory category;
  final String description;
  final Location location;
  final String address;
  final int userId;
  final double reviewScore;
  final DateTime date;
  final bool isImpulsed;
  final List<OpportunityImg> opportunityImgs;

  Opportunity({
    required this.opportunityId,
    required this.name,
    required this.price,
    required this.vacancies,
    required this.isActive,
    required this.category,
    required this.description,
    required this.location,
    required this.address,
    required this.userId,
    required this.reviewScore,
    required this.date,
    required this.isImpulsed,
    required this.opportunityImgs,
  });

  factory Opportunity.fromJson(Map<String, dynamic> json) => Opportunity(
    opportunityId: json["opportunityId"], 
    name: json["name"], 
    price: json["price"], 
    vacancies: json["vacancies"], 
    isActive: json["isActive"], 
    category: categoryFromInt(json["category"]), 
    description: json["description"], 
    location: locationFromInt(json["location"]), 
    address: json["address"], 
    userId: json["userId"], 
    reviewScore: json["reviewScore"], 
    date: DateTime.parse(json["date"]), 
    isImpulsed: json["isImpulsed"], 
    opportunityImgs: (json["opportunityImgs"] as List<dynamic>)
      .map((e) => OpportunityImg.fromJson(e))
      .toList()
  );

    Map<String, dynamic> toJson() => {
      "opportunityId": opportunityId,
      "name": name,
      "price": price,
      "vacancies": vacancies,
      "isActive": isActive,
      "category": categoryToInt(category),
      "description": description,
      "location": locationToInt(location),
      "address": address,
      "userId": userId,
      "reviewScore": reviewScore,
      "date": date.toIso8601String(),
      "isImpulsed": isImpulsed,
      "opportunityImgs": opportunityImgs.map((e) => e.toJson()).toList(),
    };

}

