import 'package:flutter/material.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:http/http.dart' as http;

class SearchState with ChangeNotifier {
  final _opportunityApiHandler = OpportunityApiHandler(http.Client());

  SearchState() {
    initializeOpportunitiesList();
  }

  List<Opportunity> _opportunitiesList = [];
  bool _isLoading = false;

  List<Opportunity>? get opportunitiesList => _opportunitiesList;
  bool get isLoading => _isLoading;

  Future<void> initializeOpportunitiesList() async {
    _isLoading = true;
    notifyListeners();

    _opportunitiesList =
        await _opportunityApiHandler.getAllOpportunities() ?? [];
    _isLoading = false;
    notifyListeners();
  }

  Future<void> search(String? keyword, Location? location, int? vacancies,
      OppCategory? category, double? minPrice, double? maxPrice) async {
    _isLoading = true;
    notifyListeners();

    _opportunitiesList = await _opportunityApiHandler.searchOpportunities(
        keyword, vacancies, minPrice, maxPrice, category, location);

    _isLoading = false;
    notifyListeners();
    return;
  }
}
