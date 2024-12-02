import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Services/payment_service.dart';

class PaymentScreen extends StatelessWidget {
  final bool isSuccess;
  final String paymentType; // e.g., "Impulse" or "Reservation"
  final VoidCallback onNavigateHome;

  const PaymentScreen({
    super.key,
    required this.isSuccess,
    required this.paymentType,
    required this.onNavigateHome,
  });

  @override
  Widget build(BuildContext context) {
    // Based on paymentType, handle the success or failure
    if (paymentType == "reservation") {
      _handleReservation();
    } else if (paymentType == "impulse") {
      _handleImpulse();
    }

    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout(context);
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout(context);
          } else {
            return _buildDesktopLayout(context);
          }
        },
      ),
    );
  }

  // For mobile layout
  Widget _buildMobileLayout(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            isSuccess ? Icons.check_circle : Icons.error,
            color: isSuccess ? Colors.green : Colors.red,
            size: 100,
          ),
          const SizedBox(height: 16),
          Text(
            isSuccess
                ? "$paymentType Pagamento Bem-Sucedido!"
                : "$paymentType Pagamento Falhou!",
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: isSuccess ? Colors.green : Colors.red,
            ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 8),
          Text(
            isSuccess
                ? "O pagamento foi efetuado com sucesso, Obrigado!"
                : "O pagamento falhou. Por favor, tente novamente.",
            style: TextStyle(fontSize: 16),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 24),
          ElevatedButton(
            onPressed: onNavigateHome,
            child: Text("Voltar ao Início"),
          ),
        ],
      ),
    );
  }

  // For tablet layout
  Widget _buildTabletLayout(BuildContext context) {
    return Center(
      child: SizedBox(
        width: 500,
        child: _buildMobileLayout(context),
      ),
    );
  }

  // For desktop layout
  Widget _buildDesktopLayout(BuildContext context) {
    return Center(
      child: SizedBox(
        width: 800,
        child: Row(
          children: [
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      isSuccess
                          ? "$paymentType Pagamento Bem-Sucedido!"
                          : "$paymentType Pagamento Falhou!",
                      style: TextStyle(
                        fontSize: 32,
                        fontWeight: FontWeight.bold,
                        color: isSuccess ? Colors.green : Colors.red,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      isSuccess
                          ? "O pagamento foi efetuado com sucesso, Obrigado!"
                          : "O pagamento falhou. Por favor, tente novamente.",
                      style: TextStyle(fontSize: 18),
                    ),
                    const SizedBox(height: 24),
                    ElevatedButton(
                      onPressed: onNavigateHome,
                      child: Text("Voltar ao Início"),
                    ),
                  ],
                ),
              ),
            ),
            Expanded(
              child: Icon(
                isSuccess ? Icons.check_circle : Icons.error,
                color: isSuccess ? Colors.green : Colors.red,
                size: 150,
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Handle Reservation
  Future<void> _handleReservation() async {
    final reservation = await PaymentService().getStoredReservation();
    if (reservation != null) {
      print("Processing reservation: $reservation");
    } else {
      print("No reservation data found!");
    }
  }

  // Handle Impulse
  Future<void> _handleImpulse() async {
    final impulse = await PaymentService().getStoredImpulse();
    if (impulse != null) {
      print("Processing impulse: $impulse");
    } else {
      print("No impulse data found!");
    }
  }

  // Extract the paymentType from the query params
  // Handle Payment Success or Cancel based on URL
  static Future<PaymentScreen> fromUri(Uri uri, VoidCallback onNavigateHome) async {
    // Determine if success or cancel page
    bool isSuccess = uri.pathSegments.contains('success');
    String paymentType = uri.queryParameters['paymentType'] ?? 'unknown';

    return PaymentScreen(
      isSuccess: isSuccess,
      paymentType: paymentType,
      onNavigateHome: onNavigateHome,
    );
  }
}
