import 'package:flutter/material.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:http/http.dart' as http;

class CreateOpportunityState with ChangeNotifier {
  final _userApiHandler = UserApiHandler(http.Client());
  final _opportunityApiHandler = OpportunityApiHandler(http.Client());

  String? _errorMessage;
  bool _isLoading = false;

  String? get errorMessage => _errorMessage;
  bool get isLoading => _isLoading;

  Future<int> getCurrentUserId() async {
    return await _userApiHandler.getStoredUserID();
  }

  // Function to register
  Future<Opportunity?> createOpportunity(
      Opportunity opportunity, List<OpportunityImg> opportunityImgs) async {
    _isLoading = true;
    notifyListeners();

    Opportunity? createdOpportunity =
        await _opportunityApiHandler.createOpportunity(opportunity);

    _isLoading = false;

    if (createdOpportunity == null) {
      _errorMessage = 'Ocorreu um erro ao criar Oportunidade';
      notifyListeners();

      return null;
    } else {
      for (OpportunityImg image in opportunityImgs) {
        createdOpportunity.opportunityImgs.add(OpportunityImg(
            imgId: 0,
            opportunityId: createdOpportunity.opportunityId,
            imageBase64: image.imageBase64));
      }
      bool isEditValid = await _opportunityApiHandler.editOpportunity(
          createdOpportunity.opportunityId, createdOpportunity);

      if (!isEditValid) {
        _errorMessage = 'Ocorreu um erro ao criar Oportunidade';
        notifyListeners();
        return null;
      }
    }

    notifyListeners();
    return createdOpportunity;
  }
}
