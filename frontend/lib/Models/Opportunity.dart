
import 'dart:ffi';

import 'package:flutter/foundation.dart';
import 'package:frontend/Enums/Category.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Models/OpportunityImg.dart';

class Opportunity {
  final int opportunityId;
  final String name;
  final Float price;
  final int vacancies;
  final bool isActive;
  final Category category;
  final String description;
  final Location location;
  final String address;
  final int userId;
  final Float reviewScore;
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
    opportunityImgs: opportunityImgs)
}

