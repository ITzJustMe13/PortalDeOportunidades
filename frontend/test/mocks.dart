import 'package:flutter/widgets.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:frontend/Services/reservation_api_handler.dart';
import 'package:frontend/Services/review_api_handler.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:frontend/State/LoginState.dart';
import 'package:mockito/annotations.dart';


@GenerateMocks([
  UserApiHandler,
  ReservationApiHandler,
  OpportunityApiHandler,
  PaymentApiHandler,
  ReviewApiHandler,
  PaymentService,
  LoginState,
  BuildContext,
  NavigatorObserver
])

void main(){}