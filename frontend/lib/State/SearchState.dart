import 'dart:math';

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
  bool _isTopChecked = false;
  bool _isLoading = false;
  String? _selectedSort;

  // Sorting options
  final Map<String, Future<void> Function(SearchState)> _sortOptions = {
    'Classificação Descendente': (SearchState searchState) => searchState.sortByScore(),
    'Preço Descendente': (SearchState searchState) => searchState.sortByPrice(),
    'Vacancies Descendente': (SearchState searchState) => searchState.sortByVacancies(),
  };

  List<Opportunity>? get opportunitiesList => _opportunitiesList;
  bool get isLoading => _isLoading;
  bool get isTopChecked => _isTopChecked;
  String? get selectedSort => _selectedSort;
  Map<String, Future<void> Function(SearchState)> get sortOptions =>
      _sortOptions;
  void setSortOption(String? sortOption) {
    _selectedSort = sortOption;
    notifyListeners();
  }

  void setSelectedSort(String? sortOption) {
    _selectedSort = sortOption;
    notifyListeners();
  }

/*TODO: search parameteres in state */

  Future<void> toggleTopChecked() async {
    _isTopChecked = !_isTopChecked;
    await search(null, null, null, null, null, null);
    notifyListeners();
  }

  Future<void> initializeOpportunitiesList() async {
    _isLoading = true;
    notifyListeners();

    _opportunitiesList =
        await _opportunityApiHandler.getAllOpportunities() ?? [];

    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners();
  }

  Future<void> search(String? keyword, Location? location, int? vacancies,
      OppCategory? category, double? minPrice, double? maxPrice) async {
    _isLoading = true;
    notifyListeners();

    _opportunitiesList = await _opportunityApiHandler.searchOpportunities(
        keyword, vacancies, minPrice, maxPrice, category, location);

    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners();
    return;
  }

  Future<void> sortByScore() async {
    _isLoading = true;
    notifyListeners(); // Notify UI that sorting is starting

    // Sort the opportunities list by score in descending order
    _opportunitiesList.sort((a, b) => b.reviewScore.compareTo(a.reviewScore));

    // Apply additional filtering if "Top" is checked
    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners(); // Notify UI that sorting is complete
  }

  Future<void> sortByPrice() async {
    _isLoading = true;
    notifyListeners(); // Notify UI that sorting is starting

    // Sort the opportunities list by score in descending order
    _opportunitiesList.sort((a, b) => b.price.compareTo(a.price));

    // Apply additional filtering if "Top" is checked
    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners(); // Notify UI that sorting is complete
  }

  Future<void> sortByVacancies() async {
    _isLoading = true;
    notifyListeners(); // Notify UI that sorting is starting

    // Sort the opportunities list by price in descending order
    _opportunitiesList.sort((a, b) => b.vacancies.compareTo(a.vacancies));

    // Apply additional filtering if "Top" is checked
    if (_isTopChecked) {
      _opportunitiesList = _opportunitiesList.where((opp) {
        return opp.isImpulsed;
      }).toList();
    }

    _isLoading = false;
    notifyListeners(); // Notify UI that sorting is complete
  }
}
